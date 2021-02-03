using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetUtilities
{
    static class DictionaryExtentions
    {
        public static void AddIfNotIn<K, V>(this Dictionary<K, V> dict, K key, V value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
        }

        public static V ReturnIfNotIn<K, V>(this Dictionary<K, V> dict, K key, V emergencyReturn)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return emergencyReturn;
        }
    }
}
