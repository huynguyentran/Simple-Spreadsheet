using System;
using System.Collections.Generic;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// The abstract representation of a parentheses.
    /// </summary>
    abstract class Parenthetical : FormulaOperator
    {
        public override object DoOperation(double[] operands)
        {
            base.DoOperation(operands);
            throw new NotImplementedException("Parenthesis can't do operations.");
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
        public override bool HandleStacks(Stack<double> values, Stack<FormulaOperator> operators, out object operationResult)
        {
            operationResult = null;
            if (DoOperationIf<Additive>(values, operators, out object additionResult)
                && !GotDouble(additionResult, values, ref operationResult))
            {
                return false;
            }

            if (operators.IsOnTop<FormulaOperator, LeftParenthesis>())
                operators.Pop();
            else
                throw new FormulaFormatException("Expected left parenthesis but found none.");

            if (DoOperationIf<Multiplicative>(values, operators, out object multiplicationResult)
                && !GotDouble(multiplicationResult, values, ref operationResult))
            {
                return false;
            }

            return true;
        }

        private bool GotDouble(object result, Stack<double> values, ref object resultHolder)
        {
            if (result is double d)
            {
                values.Push(d);
                return true;
            }
            else if (result is FormulaError e)
            {
                resultHolder = e;
                return false;
            }
            else
            {
                throw new ArgumentNullException("Expected operation to return a double or error. Got neither: " + result);
            }
        }
    }
}
