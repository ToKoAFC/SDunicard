using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSunicard.Logic.Extensions
{
    public static class StringExtension
    {
        public static string RemoveDiacritics(this string text)
        {
            return Encoding.UTF8.GetString(Encoding.GetEncoding("ISO-8859-8").GetBytes(text));
        }
    }
}
