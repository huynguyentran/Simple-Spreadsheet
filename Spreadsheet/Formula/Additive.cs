using System.Collections.Generic;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// The abstract representation of an additive operator (+,-,sum, etc.)
    /// </summary>
    abstract class Additive : FormulaOperator
    {
        /// <summary>
        /// If two additive operators are next to each other, the leftmost operator is evaluated first
        /// <para>(this condenses operations of the same precedence).
        /// </summary>
        /// <param name="values">The integer values processed thus far by the FunctionEvaluator.</param>
        /// <param name="operators">The operators processed thus far by the FunctionEvaluator.</param>
        override public void HandleStacks(Stack<int> values, Stack<FormulaOperator> operators)
        {
            DoOperationIf<Additive>(values, operators);
            operators.Push(this);
        }

        public override int GetOperandCount()
        {
            return 2;
        }
    }

    /// <summary>
    /// An addition operator "+".
    /// </summary>
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

    /// <summary>
    /// A subtraction operator "-".
    /// </summary>
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
