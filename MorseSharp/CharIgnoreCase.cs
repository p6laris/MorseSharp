using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseSharp
{
    internal class CharIgnoreCase : IEqualityComparer<char>
    {
        public bool Equals(char x, char y)
        {
            if(char.ToLower(x) == char.ToLower(y))
                return true;

            return false;
        }
           

        public int GetHashCode([DisallowNull] char obj)
        {
           return char.ToLower(obj).GetHashCode();
        }
    }
}
