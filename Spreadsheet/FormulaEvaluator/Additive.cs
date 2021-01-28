using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaEvaluator
{
    abstract class Additive : Operator
    {
        override public void HandleStacks(Stack<int> values, Stack<Operator> operators)
        {
            if (operators.IsOnTop<Operator, Additive>())
            {
                if (values.Count < 2)
                    throw new ArgumentException("Tried to perform operation, but operands were missing.");

                int operand2 = values.Pop();
                int operand1 = values.Pop();

                values.Push(operators.Pop().DoOperation(operand1, operand2));
            }

            operators.Push(this);
        }
    }
}
