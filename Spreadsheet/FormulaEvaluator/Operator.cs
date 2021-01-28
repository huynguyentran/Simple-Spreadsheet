using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaEvaluator
{
    abstract class Operator
    {
        public abstract bool IsOperator(string token);

        public abstract int DoOperation(int v1, int v2);

        public virtual void HandleStacks(Stack<int> values, Stack<Operator> operators)
        {
            operators.Push(this);
        }
    }
}
