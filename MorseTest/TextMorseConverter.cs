namespace MorseTest
{
    public class TextMorseConverterTest
    {
        [Fact]
        public void ConvertToMorseWithValidString()
        {
            var morse = Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse("The quick brown fox jumps over the lazy dog")
                .Encode();

            Assert.True(morse.Length > 0);
        }

        [Fact]
        public void ConvertToMorseWithNullString()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var morse = Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse(null)
                .Encode();
            });
        }
        [Fact]
        public void ConvertToMorseWithInvalidCharacter()
        {
            Assert.Throws<CharacterNotPresentedException>(() =>
            {
                var morse = Morse.GetConverter()
                .ForLanguage(Language.Kurdish)
                .ToMorse("~")
                .Encode();
            });
        }
        [Fact]
        public void ConvertToTextWithValidMorse()
        {
            var morse = Morse.GetConverter()
                .ForLanguage(Language.English)
                .Decode(".... ..");

            Assert.True(morse.Length > 0);
        }
        [Fact]
        public void ConvertToTextWithNullMorse()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var morse = Morse.GetConverter()
                .ForLanguage(Language.Kurdish)
                .Decode(null);
            });
        }
        [Fact]
        public void ConvertTextWithInvalidMorse()
        {
            Assert.Throws<SequenceNotFoundException>(() =>
            {
                var morse = Morse.GetConverter()
                .ForLanguage(Language.Kurdish)
                .Decode("............");
            });
        }

        [Fact]
        public void ConvertToTextWithMoreThanOneWords()
        {
            var text = Morse.GetConverter()
                .ForLanguage(Language.English)
                .Decode(".... .. / .... ..");
            Console.WriteLine("--------------------------");
            Console.WriteLine(text);
            Assert.True(text == "HI HI");

        }


    }
}
