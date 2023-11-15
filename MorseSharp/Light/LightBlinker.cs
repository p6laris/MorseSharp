namespace MorseSharp.Light
{
    internal partial class LightBlinker
    {
        private readonly int _characterSpeed;
        private readonly int _wordSpeed;

        private Action<bool> _blinkerAction;


        public LightBlinker(int characterSpeed, int wordSpeed, Action<bool> blinkerAction)
        {
            if (characterSpeed < wordSpeed)
                throw new SmallerWordSpeedException(characterSpeed, wordSpeed);

            if (blinkerAction == null)
                throw new ArgumentNullException(nameof(blinkerAction));

            _characterSpeed = characterSpeed;
            _wordSpeed = wordSpeed;
            _blinkerAction = blinkerAction;
        }
        private Task GetDotDurationAsync() => GetBlinkAsync(1.2 / _characterSpeed);

        private Task GetDashDurationAsync() => GetBlinkAsync(3.6 / _characterSpeed);

        private Task GetElementCharDurationAsync() => GetSilenceAsync(1.2 / _characterSpeed);

        private Task GetInterWordDurationAsync()
        {
            double delay = (60.0 / _wordSpeed) - (32.0 / _characterSpeed);
            double spaceLength = 7 * delay / 19;
            return GetSilenceAsync(spaceLength);
        }

        private Task GetInterCharDurationAsync()
        {
            double delay = (60.0 / _wordSpeed) - (32.0 / _characterSpeed);
            double spaceLength = 3 * delay / 19;
            return GetSilenceAsync(spaceLength);
        }
        private Task GetBlinkAsync(double seconds)
        {
            _blinkerAction?.Invoke(true);
            return Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        private Task GetSilenceAsync(double seconds)
        {
            _blinkerAction?.Invoke(false);
            return Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        private async Task GetCharacterDurations(string morseSymbol)
        {

            for (int i = 0; i < morseSymbol.Length; i++)
            {
                if (i > 0)
                    await GetElementCharDurationAsync();
                if (morseSymbol[i] == '_')
                    await GetDashDurationAsync();
                else if (morseSymbol[i] == '.')
                    await GetDotDurationAsync();
            }

        }

        public async Task BlinkLight(string morse)
        {

            var splitedText = morse.Split(' ');

            for (int i = 0; i < splitedText.Length; i++)
            {
                if (i > 0)
                    await GetInterWordDurationAsync();

                await GetCharacterDurations(splitedText[i]);
            }

            // Pad the end with a little bit of silence.
            await GetInterCharDurationAsync();
        }

    }
}
