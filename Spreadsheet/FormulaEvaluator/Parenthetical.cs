using System;
using System.Collections.Generic;

namespace FormulaEvaluator
{
    /// <summary>
    /// The abstract representation of a parentheses.
    /// </summary>
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

    /// <summary>
    /// A left parenthesis "(".
    /// </summary>
    class LeftParenthesis : Parenthetical
    {
        public override string ToString()
        {
            return "(";
        }
    }

    /// <summary>
    /// A right parenthesis ")".
    /// </summary>
    class RightParenthesis : Parenthetical
    {
        public override string ToString()
        {
            return ")";
        }

        /// <summary>
        /// A right parenthesis looks for its partner left parenthesis in the operators stack.
        /// </summary>
        /// <param name="values">The integer values processed thus far by the FunctionEvaluator.</param>
        /// <param name="operators">The operators processed thus far by the FunctionEvaluator.</param>
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
