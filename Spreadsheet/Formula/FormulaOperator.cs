using System;
using System.Collections.Generic;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// The abstract representation of an double operator.
    /// </summary>
    abstract class FormulaOperator : FormulaElement
    {
        protected override HashSet<Type> followers
        {
            get { return new HashSet<Type>() { typeof(Value), typeof(LeftParenthesis) }; }
        }

        /// <summary>
        /// Whether a token matches the operator.
        /// </summary>
        /// <param name="token">The token to recognize.</param>
        /// <returns>Whether the token matches the operator.</returns>
        public virtual bool IsOperator(string token)
        {
            return ToString().Equals(token);
        }

        /// <summary>
        /// Performs the operator's function on the number of operands the operator takes.
        /// </summary>
        /// <param name="operands">The operands of the operator.</param>
        /// <returns>The double output of the operation.</returns>
        public virtual object DoOperation(double[] operands)
        {
            if (operands.Length != GetOperandCount())
                throw new ArgumentException("Operator " + this + " takes " + GetOperandCount() + " operands, but got " + operands.Length + ".");
            return null;
        }

        // I'm overriding the ToString method to force operators to have good string representations.
        public override abstract string ToString();

        /// <summary>
        /// Gets the number of operands the operator takes.
        /// </summary>
        /// <returns>The number of operands the operator takes.</returns>
        public abstract int GetOperandCount();

        /// <summary>
        /// Processes the stacks of the FunctionEvaluator to keep operator precedence.
        /// </summary>
        /// <param name="values">The double values processed thus far by the FunctionEvaluator.</param>
        /// <param name="operators">The operators processed thus far by the FunctionEvaluator.</param>
        public virtual bool HandleStacks(Stack<double> values, Stack<FormulaOperator> operators, out object operationResult)
        {
            operationResult = null;
            operators.Push(this);
            return true;
        }

        /// <summary>
        /// Performs an operator's function on the most recent values extracted by the FunctionEvaluator.
        /// </summary>
        /// <param name="values">The double values processed thus far by the FunctionEvaluator.</param>
        /// <param name="op">The operator to evaluate with the values's operands.</param>
        public static object DoOperationWith(Stack<double> values, FormulaOperator op)
        {
            if (values.TryPops(op.GetOperandCount(), out double[] operands))
                return op.DoOperation(operands);
            else
                throw new ArgumentException("Tried to perform operation, but operands were missing.");
        }

        /// <summary>
        /// Performs an operation on the most recent values extracted by the FunctionEvaluator 
        /// <para>if the most recent operator extracted by FunctionEvaluator matches the desired operation.
        /// </summary>
        /// <typeparam name="T">The type of operator to look for.</typeparam>
        /// <param name="values">The double values processed thus far by the FunctionEvaluator.</param>
        /// <param name="operators">The operators processed thus far by the FunctionEvaluator.</param>
        public static bool DoOperationIf<T>(Stack<double> values, Stack<FormulaOperator> operators, out object operationResult) where T: FormulaOperator
        {
            operationResult = null;

            if (operators.IsOnTop<FormulaOperator, T>())
            {
                operationResult = DoOperationWith(values, operators.Pop());
                return true; 
            }
            return false;
        }
    }
}
