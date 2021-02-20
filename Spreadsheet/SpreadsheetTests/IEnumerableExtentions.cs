using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetTests
{
    public static class IEnumerableExtentions
    {
        public static bool Contains<T>(this IEnumerable<T> enumerable, T search)
        {
            foreach (T item in enumerable)
                if (search.Equals(item))
                    return true;
            return false;
        }
    }
}
