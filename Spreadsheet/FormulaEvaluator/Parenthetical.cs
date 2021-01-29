using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaEvaluator
{
    abstract class Parenthetical : Operator
    {
        public override int DoOperation(int[] operands)
        {
            base.DoOperation(operands);
            throw new NotImplementedException();
        }

        public override int GetOperandCount()
        {
            return 0;
        }
    }

    class LeftParenthesis : Parenthetical
    {
        public override string ToString()
        {
            return "(";
        }
    }

    class RightParenthesis : Parenthetical
    {
        public override string ToString()
        {
            return ")";
        }

        public override void HandleStacks(Stack<int> values, Stack<Operator> operators)
        {
            DoOperationIf<Additive>(values, operators);

            if (operators.IsOnTop<Operator, LeftParenthesis>())
                operators.Pop();
            else
                throw new ArgumentException("Expected left parenthesis but found none.");

            DoOperationIf<Multiplicative>(values, operators);
        }
    }
}
