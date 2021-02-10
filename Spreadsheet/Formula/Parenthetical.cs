using System;
using System.Collections.Generic;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// The abstract representation of a parentheses.
    /// </summary>
    abstract class Parenthetical : FormulaOperator
    {
        public override double DoOperation(double[] operands)
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
        protected override HashSet<Type> followers
        {
            get { return new HashSet<Type>() { typeof(Additive), typeof(Multiplicative) }; }
        }
        public override string ToString()
        {
            return ")";
        }

        /// <summary>
        /// A right parenthesis looks for its partner left parenthesis in the operators stack.
        /// </summary>
        /// <param name="values">The integer values processed thus far by the FunctionEvaluator.</param>
        /// <param name="operators">The operators processed thus far by the FunctionEvaluator.</param>
        public override void HandleStacks(Stack<double> values, Stack<FormulaOperator> operators)
        {
            DoOperationIf<Additive>(values, operators);

            if (operators.IsOnTop<FormulaOperator, LeftParenthesis>())
                operators.Pop();
            else
                throw new ArgumentException("Expected left parenthesis but found none.");

            DoOperationIf<Multiplicative>(values, operators);
        }
    }
}
