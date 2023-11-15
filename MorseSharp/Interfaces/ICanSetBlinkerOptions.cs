using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseSharp.Interfaces
{
    public interface ICanSetBlinkerOptions
    {
        ICanConvertToLight SetBlinkerOptions(int charSpeed, int wordSpeed);
    }
}
