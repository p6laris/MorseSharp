using System.Diagnostics;

namespace MorseSharp.Light;

/// <summary>
/// Plays a recorded element stream in real time through an on/off callback.
/// </summary>
internal static class LightBlinker
{
    /// <summary>
    /// Plays <paramref name="count"/> elements from <paramref name="elements"/> (rented from <see cref="ArrayPool{T}.Shared"/>,
    /// returned when done) and switches the light off if cancelled mid-sequence.
    /// </summary>
    /// <remarks>
    /// Every element is scheduled against an absolute target time measured from the start, so the coarse resolution of
    /// <see cref="Task.Delay(TimeSpan, CancellationToken)"/> (about 15 ms on Windows) does not accumulate over a long message.
    /// The continuation deliberately keeps the caller's synchronization context so UI code can update controls in the callback.
    /// </remarks>
    public static async Task BlinkAsync(byte[] elements, int count, MorseTiming timing, Action<bool> action, CancellationToken cancellationToken)
    {
        try
        {
            long start = Stopwatch.GetTimestamp();
            double targetSeconds = 0;

            for (int i = 0; i < count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                bool on;
                double seconds;
                switch (elements[i])
                {
                    case ElementRecorder.DotElement: on = true; seconds = timing.Dot; break;
                    case ElementRecorder.DashElement: on = true; seconds = timing.Dash; break;
                    case ElementRecorder.ElementGapElement: on = false; seconds = timing.ElementGap; break;
                    case ElementRecorder.CharGapElement: on = false; seconds = timing.CharGap; break;
                    default: on = false; seconds = timing.WordGap; break;
                }

                action(on);

                targetSeconds += seconds;
                TimeSpan remaining = TimeSpan.FromSeconds(targetSeconds) - Stopwatch.GetElapsedTime(start);
                if (remaining > TimeSpan.Zero)
                    await Task.Delay(remaining, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            action(false);
            throw;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(elements);
        }
    }
}
