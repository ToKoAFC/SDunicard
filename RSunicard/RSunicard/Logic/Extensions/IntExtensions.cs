using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSunicard.Logic.Extensions
{
    public static class IntExtension
    {
        public static string ToTwoDigitString(this int number)
        {
            if (number > 9)
                return number.ToString();

            return $"0{number}";
        }
    }
}
