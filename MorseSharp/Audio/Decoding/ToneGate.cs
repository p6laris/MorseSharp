namespace MorseSharp.Audio.Decoding;

/// <summary>
/// Turns per-block energies into key-down and key-up decisions, given a separating level.
/// </summary>
/// <remarks>
/// Two levels rather than one, because a single cutoff chatters: a block sitting right on it flips the key on and
/// off and shatters one dash into several dots. The gate opens above the level and only closes well below it, so a
/// decision has to be committed to before it can be reversed.
/// </remarks>
internal struct ToneGate
{
    /// <summary>Opens a little above the split point.</summary>
    private const float OpenFactor = 1.10f;

    /// <summary>Closes well below it, which is what gives the hysteresis.</summary>
    private const float CloseFactor = 0.70f;

    private readonly float _openLevel;
    private readonly float _closeLevel;
    private bool _keyDown;

    /// <summary>Creates a gate around a separating energy level.</summary>
    public ToneGate(float threshold)
    {
        _openLevel = threshold * OpenFactor;
        _closeLevel = threshold * CloseFactor;
        _keyDown = false;
    }

    /// <summary>Feeds one block's energy in and returns whether the key is down.</summary>
    public bool Update(float power)
    {
        if (_keyDown)
        {
            if (power < _closeLevel)
                _keyDown = false;
        }
        else if (power > _openLevel)
        {
            _keyDown = true;
        }

        return _keyDown;
    }
}
