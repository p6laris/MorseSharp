using ConsoleExample.Demos;

try
{
    EncodingDemo.Run();
    ProsignDemo.Run();
    CustomAlphabetDemo.Run();
    AudioFormatDemo.Run();
    AudioDecodingDemo.Run();
    StreamingDecodingDemo.Run();
    ElementStreamDemo.Run();
    KeyerDemo.Run();
    KochDemo.Run();
    CallsignAndQsoDemo.Run();
    await LightBlinkDemo.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
