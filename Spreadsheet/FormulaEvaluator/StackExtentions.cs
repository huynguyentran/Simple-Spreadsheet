using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaEvaluator
{
    static class StackExtentions
    {
        public static bool IsOnTop<T,S>(this Stack<T> s) where S : T
        {
            if (s.Count < 1)
                return false;
            return s.Peek() is S;
        }

        public static bool TryPop<T>(this Stack<T> s, out T item)
        {
            item = default(T);

            if (s.Count < 1)
                return false;

            item = s.Pop();
            return true;
        }

        public static bool TryPops<T>(this Stack<T> s, int times, out T[] items)
        {
            items = new T[times];
            for (int i = 0; i < times; i++)
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
