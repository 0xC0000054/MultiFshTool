using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace loaddatfsh
{
    internal static class StringExtensions
    {
        internal static bool Contains(this string s, string value, StringComparison comparisonType)
        {
            return s.IndexOf(value, comparisonType) >= 0;
        }
    }
}
