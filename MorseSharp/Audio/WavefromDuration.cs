namespace MorseSharp.Audio;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct WaveformDurations 
{
    public readonly int Dot, Dash, EleChar, InterChar, InterWord;
    public WaveformDurations(int dot, int dash, int eleChar, int interChar, int interWord)
    {
        Dot = dot;
        Dash = dash;
        EleChar = eleChar;
        InterChar = interChar;
        InterWord = interWord;
    }
}
