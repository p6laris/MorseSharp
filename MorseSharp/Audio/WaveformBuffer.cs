namespace MorseSharp.Audio
{
    internal readonly ref struct WaveformBuffer : IDisposable
    {
        private readonly SpanOwner<short> _buffer;

        private readonly WaveformOffsets _waveformOffsets;
        private readonly WaveformDurations _waveformDurations;

        private readonly double _angleIncrement;

        public WaveformBuffer(int characterSpeed, int wordSpeed, double frequency)
        {
            double delay = (60.0 / wordSpeed) - (32.0 / characterSpeed);

            _angleIncrement = 2 * Math.PI * frequency / 11025;
            _waveformDurations = new WaveformDurations(
                dot: (int)(11025 * (1.2 / characterSpeed)),
                dash: (int)(11025 * (3.6 / characterSpeed)),
                eleChar: (int)(11025 * (1.2 / characterSpeed)),
                interChar: (int)(11025 * (3 * delay / 19)),
                interWord: (int)(11025 * (7 * delay / 19))
            );

            int totalSamples = _waveformDurations.Dot
                               + _waveformDurations.Dash
                               + _waveformDurations.EleChar
                               + _waveformDurations.InterChar
                               + _waveformDurations.InterWord;

            _buffer = SpanOwner<short>.Allocate(totalSamples, AllocationMode.Clear);

            _waveformOffsets = new WaveformOffsets(
                dot: 0,
                dash: _waveformDurations.Dot,
                eleChar: _waveformDurations.Dot + _waveformDurations.Dash,
                interChar: _waveformDurations.Dot + _waveformDurations.Dash + _waveformDurations.EleChar,
                interWord: _waveformDurations.Dot + _waveformDurations.Dash + _waveformDurations.EleChar + _waveformDurations.InterChar
            );

            PopulateWave(GetDot());
            PopulateWave(GetDash());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<short> GetDot() => _buffer.Span.Slice(_waveformOffsets.Dot, _waveformDurations.Dot);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<short> GetDash() => _buffer.Span.Slice(_waveformOffsets.Dash, _waveformDurations.Dash);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<short> GetEleCharSpace() => _buffer.Span.Slice(_waveformOffsets.EleChar, _waveformDurations.EleChar);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<short> GetInterCharSpace() => _buffer.Span.Slice(_waveformOffsets.InterChar, _waveformDurations.InterChar);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<short> GetInterWordSpace() => _buffer.Span.Slice(_waveformOffsets.InterWord, _waveformDurations.InterWord);

        private void PopulateWave(Span<short> data)
        {
            int samples = data.Length;
            double angle = 0.0;
            double sinVal = Math.Sin(angle);
            double cosVal = Math.Cos(angle);
            double cosIncrement = Math.Cos(_angleIncrement);
            double sinIncrement = Math.Sin(_angleIncrement);

            for (int i = 0; i < samples; i++)
            {
                data[i] = (short)(32760 * sinVal);

                // Compute next sine wave using trigonometric identities
                (sinVal, cosVal) = (sinVal * cosIncrement + cosVal * sinIncrement,
                    cosVal * cosIncrement - sinVal * sinIncrement);
            }
        }

        public void Dispose()
        {
            _buffer.Dispose();
        }
    }
}
