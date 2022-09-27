using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseSharp.MorseRepo
{
    internal interface IMorseConverter
    {
        Task<string> ConvertToMorse(string text);
    }
}
