using System;

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
