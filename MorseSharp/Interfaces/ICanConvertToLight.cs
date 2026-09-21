namespace MorseSharp.Interfaces;

/// <summary>
/// Terminal step of the light chain: plays the current Morse sequence as on/off callbacks in real time.
/// </summary>
public interface ICanConvertToLight
{
    /// <summary>
    /// Plays the sequence, invoking <paramref name="blinkerAction"/> with <c>true</c> at the start of every dot or dash
    /// and <c>false</c> at the start of every gap. The final callback is always <c>false</c>.
    /// </summary>
    /// <remarks>
    /// Timing is drift-compensated: each element is scheduled against the wall clock, so timer jitter does not accumulate.
    /// Callbacks run on the captured synchronization context (for example the UI thread when awaited from a UI event handler).
    /// </remarks>
    /// <param name="blinkerAction">Receives <c>true</c> for light on and <c>false</c> for light off.</param>
    /// <param name="cancellationToken">Stops the sequence early; the light is switched off before the task is cancelled.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="blinkerAction"/> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task DoBlinks(Action<bool> blinkerAction, CancellationToken cancellationToken = default);
}
