using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetTests
{
    /// <author>William Erignac</author>
    /// <version>2/19/2021</version>
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
