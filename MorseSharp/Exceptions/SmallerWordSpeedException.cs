using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseSharp.Exceptions
{
    [Serializable]
    internal class SmallerWordSpeedException : Exception
    {
        public SmallerWordSpeedException() { }

        public SmallerWordSpeedException(string message) : base(message)
        {

        }
        public SmallerWordSpeedException(int charSpeed, int wordSpeed)
            : base($"The character speed must not be smaller than word speed: {charSpeed} < {wordSpeed}")
        {

        }
    }
}
