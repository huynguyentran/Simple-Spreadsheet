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

        public static void DoOperationWith(Stack<int> values, Operator op)
        {
            if (values.TryPops(2, out int[] operands))
                values.Push(op.DoOperation(operands[1], operands[0]));
            else
                throw new ArgumentException("Tried to perform operation, but operands were missing.");
        }

        public static void DoOperationIf<T>(Stack<int> values, Stack<Operator> operators) where T: Operator
        {
            if (operators.IsOnTop<Operator, T>())
                DoOperationWith(values, operators.Pop());
        }
    }
}
