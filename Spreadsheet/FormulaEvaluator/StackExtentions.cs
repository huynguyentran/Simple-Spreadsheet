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
    }
}
