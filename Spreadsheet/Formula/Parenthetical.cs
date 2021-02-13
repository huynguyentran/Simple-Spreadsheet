using System;
using System.Collections.Generic;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// The abstract representation of a parentheses.
    /// </summary>
    abstract class Parenthetical : FormulaOperator
    {
        //Do operation should never be called on a parenthesis.
        public override object DoOperation(double[] operands)
        {
            throw new NotImplementedException("Parenthesis can't do operations.");
        }

        public override int GetOperandCount()
        {
            return 0;
        }

        /// <summary>
        /// This handle stacks is used in formula construction to check for a correct
        /// number of parentheses.
        /// </summary>
        /// <param name="operators">The operators stack.</param>
        public abstract void HandleStacks(Stack<FormulaOperator> operators);
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

        public override void HandleStacks(Stack<FormulaOperator> operators)
        {
            operators.Push(this);
        }
    }

    /// <summary>
    /// A right parenthesis ")".
    /// </summary>
    class RightParenthesis : Parenthetical
    {
        protected override HashSet<Type> followers
        {
            get { return new HashSet<Type>() { typeof(Additive), typeof(Multiplicative), typeof(RightParenthesis) }; }
        }
        public override string ToString()
        {
            return ")";
        }

        /// <summary>
        /// A right parenthesis looks for its partner left parenthesis in the operators stack.
        /// It also initiates interior additive operations and exterior multiplicative operations.
        /// </summary>
        public override bool HandleStacks(Stack<double> values, Stack<FormulaOperator> operators, out OperatorError operationResult)
        {
            operationResult = null;
            if (DoOperationIf<Additive>(values, operators, out object additionResult)
                && !GotDouble(additionResult, values, ref operationResult))
            {
                return false;
            }
            //Check for left parenthesis.
            HandleStacks(operators);

            if (DoOperationIf<Multiplicative>(values, operators, out object multiplicationResult)
                && !GotDouble(multiplicationResult, values, ref operationResult))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for a left parenthesis in the operator stack.
        /// </summary>
        /// <param name="operators">The operator stack.</param>
        public override void HandleStacks(Stack<FormulaOperator> operators)
        {
            if (operators.IsOnTop<FormulaOperator, LeftParenthesis>())
                operators.Pop();
            else
                throw new FormulaFormatException("Expected left parenthesis but found none.");
        }
    }
}
