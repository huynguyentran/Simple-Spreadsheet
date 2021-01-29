using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaEvaluator
{
    abstract class Operator
    {
        public virtual bool IsOperator(string token)
        {
            return ToString().Equals(token);
        }

        public virtual int DoOperation(int[] operands)
        {
            if (operands.Length != GetOperandCount())
                throw new ArgumentException("Operator " + this + " takes " + GetOperandCount() + " operands, but got " + operands.Length + ".");
            return 1;
        }

        public override abstract string ToString();

        public abstract int GetOperandCount();

        public virtual void HandleStacks(Stack<int> values, Stack<Operator> operators)
        {
            operators.Push(this);
        }

        public static void DoOperationWith(Stack<int> values, Operator op)
        {
            if (values.TryPops(op.GetOperandCount(), out int[] operands))
                values.Push(op.DoOperation(operands));
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
