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

        public override int GetOperandCount()
        {
            return 2;
        }
    }

    class Plus : Additive
    {
        override public int DoOperation (int[] operands)
        {
            base.DoOperation(operands);
            return operands[0] + operands[1];
        }

        public override string ToString()
        {
            return "+";
        }
    }

    class Minus : Additive
    {
        override public int DoOperation(int[] operands)
        {
            base.DoOperation(operands);
            return operands[0] - operands[1];
        }

        public override string ToString()
        {
            return "-";
        }
    }
}
