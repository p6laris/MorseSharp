namespace MorseSharp;

/// <summary>
/// Turns paddle presses into correctly timed Morse elements.
/// </summary>
/// <remarks>
/// <para>
/// A paddle is two contacts: one asks for dots, the other for dashes. Holding one sends that element over and over;
/// squeezing both alternates between them, which is what makes the keying iambic. The keyer decides what comes next
/// and how long it lasts, so the operator supplies rhythm rather than timing.
/// </para>
/// <para>
/// There is no clock in here. <see cref="Paddles"/> reports the contacts as they are now, <see cref="TryRead"/> hands
/// back the next element once the previous one has been played, and <see cref="MorseElement.Duration"/> says how long
/// to play it for. That keeps the keying rules testable without waiting for real time, and lets the caller drive a
/// sidetone, a light or a transmitter with whatever timer it already has.
/// </para>
/// <para>
/// <see cref="Paddles"/> is meant to be called from wherever the hardware is polled, which is rarely the thread that
/// plays the elements, so the two are safe to call concurrently. While an element is in progress the opposite contact
/// is remembered as soon as it closes, so a tap shorter than that element is still heard.
/// </para>
/// </remarks>
public sealed class IambicKeyer
{
    private const int DotDown = 1;
    private const int DashDown = 2;
    private const int DotLatched = 4;
    private const int DashLatched = 8;
    private const int GapPending = 16;
    private const int SendingDot = 32;
    private const int SendingDash = 64;

    private const int Contacts = DotDown | DashDown;
    private const int Latched = DotLatched | DashLatched;
    private const int Sending = SendingDot | SendingDash;

    private readonly KeyerMode _mode;
    private readonly MorseTiming _timing;

    private int _paddles;
    private bool _lastWasDash = true;

    /// <summary>Creates a keyer.</summary>
    /// <param name="wordsPerMinute">Keying speed (PARIS standard).</param>
    /// <param name="mode">What a released squeeze does; see <see cref="KeyerMode"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="wordsPerMinute"/> is not positive.</exception>
    public IambicKeyer(int wordsPerMinute = 25, KeyerMode mode = KeyerMode.B)
    {
        _timing = new MorseTiming(wordsPerMinute, wordsPerMinute);
        _mode = mode;
    }

    /// <summary>The mode this keyer was created with.</summary>
    public KeyerMode Mode => _mode;

    /// <summary>Whether the keyer has nothing left to send for the paddles as they stand.</summary>
    public bool IsIdle => (Volatile.Read(ref _paddles) & (Contacts | Latched | GapPending)) == 0;

    /// <summary>
    /// Reports the state of the two contacts. Call it whenever they change, or on every poll; only the levels matter.
    /// </summary>
    /// <param name="dot">Whether the dot contact is closed.</param>
    /// <param name="dash">Whether the dash contact is closed.</param>
    public void Paddles(bool dot, bool dash)
    {
        while (true)
        {
            int current = Volatile.Read(ref _paddles);
            int desired = (current & (Latched | GapPending | Sending)) | (dot ? DotDown : 0) | (dash ? DashDown : 0);

            // Only the contact opposite the element in progress is remembered, which is what makes a squeeze
            // alternate. Remembering the one already being sent would turn letting go early into an extra element.
            // While idle neither is in progress, so a tap on either is kept.
            if (dot && (current & SendingDot) == 0)
                desired |= DotLatched;

            if (dash && (current & SendingDash) == 0)
                desired |= DashLatched;

            if (Interlocked.CompareExchange(ref _paddles, desired, current) == current)
                return;
        }
    }

    /// <summary>
    /// Returns the next element to play, or <c>false</c> when the paddles are asking for nothing.
    /// </summary>
    /// <remarks>
    /// Elements come out as a dot or a dash followed by the gap that separates it from the next one. A keyer never
    /// produces character or word gaps: it cannot know that a character has ended, only that the operator has paused.
    /// Pass the elements and those pauses to a <see cref="KeyerDecoder"/> to get text back.
    /// </remarks>
    /// <param name="element">The element to play, for <see cref="MorseElement.Duration"/>.</param>
    public bool TryRead(out MorseElement element)
    {
        while (true)
        {
            int current = Volatile.Read(ref _paddles);

            if ((current & GapPending) != 0)
            {
                if (Interlocked.CompareExchange(ref _paddles, current & ~GapPending, current) != current)
                    continue;

                element = Element(MorseElementKind.ElementGap);
                return true;
            }

            // The whole of the difference between the modes. With both contacts open the operator has let go, and
            // mode A drops what was remembered and stops there; mode B honours it, which is the one extra element.
            int effective = _mode == KeyerMode.A && (current & Contacts) == 0 ? current & ~Latched : current;

            bool wantDot = (effective & (DotDown | DotLatched)) != 0;
            bool wantDash = (effective & (DashDown | DashLatched)) != 0;

            if (!wantDot && !wantDash)
            {
                if (Interlocked.CompareExchange(ref _paddles, current & ~(Latched | Sending), current) != current)
                    continue;

                // Nothing in progress, so the next squeeze starts on a dot as an operator expects.
                _lastWasDash = true;
                element = default;
                return false;
            }

            // Both wanted means alternate; that is what iambic keying is.
            bool dash = wantDot && wantDash ? !_lastWasDash : wantDash;
            int desired = (current & ~(Latched | Sending)) | GapPending | (dash ? SendingDash : SendingDot);

            if (Interlocked.CompareExchange(ref _paddles, desired, current) != current)
                continue;

            _lastWasDash = dash;
            element = Element(dash ? MorseElementKind.Dash : MorseElementKind.Dot);
            return true;
        }
    }

    private MorseElement Element(MorseElementKind kind) => new(kind, _timing.DurationOf(kind));
}
