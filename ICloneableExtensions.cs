using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace loaddatfsh
{
    static class ICloneableExtensions
    {
        public static T Clone<T>(this ICloneable obj)
        {
            return (T)obj.Clone();
        }
    }
}
