using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaEvaluator
{
    abstract class Parenthetical : Operator
    {
    }

    class LeftParenthesis : Parenthetical
    {
        public override int DoOperation(int v1, int v2)
        {
            throw new NotImplementedException();
        }

        public override bool IsOperator(string token)
        {
            return "(".Equals(token);
        }
    }

    class RightParenthesis : Parenthetical
    {
        public override int DoOperation(int v1, int v2)
        {
            throw new NotImplementedException();
        }

        public override bool IsOperator(string token)
        {
            return ")".Equals(token);
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
