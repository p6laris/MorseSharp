using System.Threading.Tasks.Sources;

namespace MorseSharp.Core;

/// <summary>
/// Plays a <see cref="MorseElementSequence"/> in real time as an async stream.
/// </summary>
/// <remarks>
/// <para>
/// Written by hand rather than with <c>async IAsyncEnumerable</c> so that a long sequence costs a fixed number of
/// allocations instead of a <see cref="Task"/>, a timer entry and a cancellation registration per element. The same
/// object is the enumerable, the enumerator and the value task source; one <see cref="ITimer"/> is created on first
/// use and re-armed for every element after that.
/// </para>
/// <para>
/// Each element is scheduled against an absolute target measured from the start, so timer jitter does not accumulate
/// over a long message. Continuations keep the caller's synchronization context, so a light callback driven from this
/// runs on the UI thread when it was awaited from one.
/// </para>
/// </remarks>
internal sealed class MorseElementStream : IAsyncEnumerable<MorseElement>, IAsyncEnumerator<MorseElement>, IValueTaskSource<bool>
{
    private readonly MorseElementSequence _sequence;
    private readonly TimeProvider _time;
    private readonly CancellationToken _outerToken;

    private ManualResetValueTaskSourceCore<bool> _core;
    private MorseElementEnumerator _enumerator;
    private CancellationTokenSource? _linked;
    private CancellationTokenRegistration _registration;
    private CancellationToken _token;
    private ITimer? _timer;
    private long _start;
    private TimeSpan _target;
    private int _pending;
    private int _taken;

    public MorseElementStream(MorseElementSequence sequence, TimeProvider time, CancellationToken cancellationToken)
    {
        _sequence = sequence;
        _time = time;
        _outerToken = cancellationToken;
        _core.RunContinuationsAsynchronously = true;
    }

    /// <inheritdoc />
    public MorseElement Current { get; private set; }

    /// <inheritdoc />
    public IAsyncEnumerator<MorseElement> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => Interlocked.Exchange(ref _taken, 1) == 0
            ? Start(cancellationToken)
            : new MorseElementStream(_sequence, _time, _outerToken).Start(cancellationToken);

    /// <inheritdoc />
    public ValueTask<bool> MoveNextAsync()
    {
        if (_token.IsCancellationRequested)
            return ValueTask.FromCanceled<bool>(_token);

        // The element already handed over is still playing; wait out the rest of it before producing the next one.
        TimeSpan remaining = _target - _time.GetElapsedTime(_start);
        if (remaining <= TimeSpan.Zero)
            return new ValueTask<bool>(Advance());

        _core.Reset();
        Volatile.Write(ref _pending, 1);
        _timer ??= _time.CreateTimer(static state => ((MorseElementStream)state!).OnTimer(), this,
            Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        _timer.Change(remaining, Timeout.InfiniteTimeSpan);

        return new ValueTask<bool>(this, _core.Version);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _registration.Dispose();
        _timer?.Dispose();
        _linked?.Dispose();
        return ValueTask.CompletedTask;
    }

    private MorseElementStream Start(CancellationToken cancellationToken)
    {
        // Mirrors what [EnumeratorCancellation] does for a compiler-generated iterator: honour both the token given to
        // the method and the one given to the foreach, without linking them when only one can actually be cancelled.
        if (!_outerToken.CanBeCanceled)
            _token = cancellationToken;
        else if (!cancellationToken.CanBeCanceled || cancellationToken == _outerToken)
            _token = _outerToken;
        else
            _token = (_linked = CancellationTokenSource.CreateLinkedTokenSource(_outerToken, cancellationToken)).Token;

        _enumerator = _sequence.GetEnumerator();
        _start = _time.GetTimestamp();

        if (_token.CanBeCanceled)
            _registration = _token.UnsafeRegister(static state => ((MorseElementStream)state!).OnCancelled(), this);

        return this;
    }

    private bool Advance()
    {
        if (!_enumerator.MoveNext())
            return false;

        Current = _enumerator.Current;
        _target += Current.Duration;
        return true;
    }

    private void OnTimer()
    {
        // Whichever of the timer and the cancellation gets here first owns the completion.
        if (Interlocked.Exchange(ref _pending, 0) == 0)
            return;

        try
        {
            _core.SetResult(Advance());
        }
        catch (Exception exception)
        {
            _core.SetException(exception);
        }
    }

    private void OnCancelled()
    {
        if (Interlocked.Exchange(ref _pending, 0) == 0)
            return;

        _core.SetException(new OperationCanceledException(_token));
    }

    bool IValueTaskSource<bool>.GetResult(short token) => _core.GetResult(token);

    ValueTaskSourceStatus IValueTaskSource<bool>.GetStatus(short token) => _core.GetStatus(token);

    void IValueTaskSource<bool>.OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        => _core.OnCompleted(continuation, state, token, flags);
}
