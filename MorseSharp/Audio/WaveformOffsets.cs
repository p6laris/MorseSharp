namespace MorseSharp.Audio;

[StructLayout(LayoutKind.Sequential)]
 internal readonly ref struct WaveformOffsets
 {
     public readonly int Dot, Dash, EleChar, InterChar, InterWord;
     public WaveformOffsets(int dot, int dash, int eleChar, int interChar, int interWord)
     {
         Dot = dot;
         Dash = dash;
         EleChar = eleChar;
         InterChar = interChar;
         InterWord = interWord;
     }
 }
