using System.Collections.Generic;

namespace FormulaEvaluator
{
    /// <summary>
    /// A class that holdes a few stack extentions.
    /// </summary>
    static class StackExtentions
    {
        /// <summary>
        /// Checks if the topmost item of a stack is of a desired type.
        /// </summary>
        /// <typeparam name="T">The type of items in the stack.</typeparam>
        /// <typeparam name="S">The type to look for in the stack.</typeparam>
        /// <param name="s">The stack.</param>
        /// <returns>Whether the top item in the stack is of the desired type.</returns>
        public static bool IsOnTop<T,S>(this Stack<T> s) where S : T
        {
            if (s.Count < 1)
                return false;
            return s.Peek() is S;
        }

        /// <summary>
        /// Tries to pop an item from a stack.
        /// </summary>
        /// <typeparam name="T">The type of a stack.</typeparam>
        /// <param name="s">The stack.</param>
        /// <param name="item">The item popped out of the stack.</param>
        /// <returns>Whether an item was successfully popped.</returns>
        public static bool TryPop<T>(this Stack<T> s, out T item)
        {
            item = default(T);

            if (s.Count < 1)
                return false;

            item = s.Pop();
            return true;
        }

        /// <summary>
        /// Tries to pop multiple items from a stack.
        /// </summary>
        /// <typeparam name="T">The type of the stack.</typeparam>
        /// <param name="s">The stack.</param>
        /// <param name="times">How many items to pop from the stack.</param>
        /// <param name="items">The items popped from the stack.</param>
        /// <returns>Whether all the items were sucessfully popped.</returns>
        public static bool TryPops<T>(this Stack<T> s, int times, out T[] items)
        {
            items = new T[times];
            for (int i = times - 1; i >= 0; i--)
            {
                if (s.TryPop(out T item))
                    items[i] = item;
                else
                    return false;
            }

            return true;
        }
    }
}
