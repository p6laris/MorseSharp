using MorseSharp;


try
{
    //Encoding
    var morse = Morse.GetConverter()
        .ForLanguage(Language.English)
        .ToMorse("Hi")
        .Encode();

    //Decoding
    var text = Morse.GetConverter()
        .ForLanguage(Language.English)
        .Decode(".... ..");

    //Light Blinking
    await Morse.GetConverter()
        .ForLanguage(Language.English)
        .ToLight(".... ..")
        .SetBlinkerOptions(25, 25)
        .DoBlinks((hasToBlink) =>
        {
            if (hasToBlink)
                Console.BackgroundColor = ConsoleColor.White;
            else 
                Console.BackgroundColor = ConsoleColor.Black;
        });

}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}
