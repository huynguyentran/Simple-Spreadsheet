using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaEvaluator
{
    abstract class Additive : Operator
    {
        override public void HandleStacks(Stack<int> values, Stack<Operator> operators)
        {
            DoOperationIf<Additive>(values, operators);
            operators.Push(this);
        }
    }

    class Plus : Additive
    {
        override public int DoOperation (int operand1, int operand2)
        {
            return operand1 + operand2;
        }

        public override bool IsOperator(string token)
        {
            return "+".Equals(token);
        }
    }

    class Minus : Additive
    {
        override public int DoOperation(int operand1, int operand2)
        {
            return operand1 - operand2;
        }

        public override bool IsOperator(string token)
        {
            return "-".Equals(token);
        }
    }
}
