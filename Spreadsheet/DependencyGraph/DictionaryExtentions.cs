using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// A class that adds some extention methods to dictionaries for the DependencyGraph class.
    /// </summary>
    static class DictionaryExtentions
    {
        /// <summary>
        /// Adds an key,value pair to a dictionary if the key is not already
        /// in the dictionary.
        /// </summary>
        /// <typeparam name="K">The key type.</typeparam>
        /// <typeparam name="V">The value type.</typeparam>
        /// <param name="dict">The dictionary.</param>
        /// <param name="key">The key of the pair to be added.</param>
        /// <param name="value">The value of the pair to be added.</param>
        public static void AddIfNotIn<K, V>(this Dictionary<K, V> dict, K key, V value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
        }

        /// <summary>
        /// Returns the value mapped to the key, or returns the default if
        /// the key is not in the dictionary.
        /// </summary>
        /// <typeparam name="K">The key type.</typeparam>
        /// <typeparam name="V">The value type.</typeparam>
        /// <param name="dict">The dictionary.</param>
        /// <param name="key">The key of the value to be retrieved.</param>
        /// <param name="defaultValue">The default value if the key is not in the dictionary.</param>
        /// <returns>Either the value mapped to the key, or the default value.</returns>
        public static V GetOrDefault<K, V>(this Dictionary<K, V> dict, K key, V defaultValue)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return defaultValue;
        }
    }
}
