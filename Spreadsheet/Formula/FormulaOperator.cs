using System;
using System.Collections.Generic;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// The abstract representation of an double operator.
    /// </summary>
    abstract class FormulaOperator : FormulaElement
    {
        /// <summary>
        /// Most operators can come before a value or a left parenthesis.
        /// </summary>
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
        /// <returns>The output of the operation, a double or an OperatorError.</returns>
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
        /// Error cannot be anything but an OperatorError or null.
        /// </summary>
        /// <param name="values">The double values processed thus far by the FunctionEvaluator.</param>
        /// <param name="operators">The operators processed thus far by the FunctionEvaluator.</param>
        /// <param name = "error">An error caused by the operation or null.</param>
        /// <returns>If the operator successfully handeled the stacks (i.e. Were there any errors?).</returns>
        public virtual bool HandleStacks(Stack<double> values, Stack<FormulaOperator> operators, out OperatorError error)
        {
            error = null;
            operators.Push(this);
            return true;
        }

        /// <summary>
        /// Performs an operator's function on the most recent values extracted by the FunctionEvaluator.
        /// </summary>
        /// <param name="values">The double values processed thus far by the FunctionEvaluator.</param>
        /// <param name="op">The operator to evaluate with the values's operands.</param>
        /// <returns>A double, null, or a FormulaError.</returns>
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
        /// <paramref name="operationResult"/>The result of the operation.</param>
        /// <returns>Whether the requested operator was on the top of the stack.</returns>
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

        /// <summary>
        /// Checks whether the output of a operation is a double, error, or null and responds accordingly.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        /// <param name="values">The values to push the result onto if it is a double.</param>
        /// <param name="error">The error, if an error results from the operation.</param>
        /// <returns>True if the result is a double;
        ///  false if the result is an error;
        ///  an exception is thrown if the result is null or anything else.</returns>
        public static bool GotDouble(object result, Stack<double> values, ref OperatorError error)
        {
            if (result is double d)
            {
                values.Push(d);
                return true;
            }
            else if (result is FormulaError e)
            {
                error = new OperatorError(e);
                return false;
            }
            else
            {
                throw new ArgumentNullException("Expected operation to return a double or error. Got neither: " + result);
            }
        }
    }

    /// <summary>
    /// A wrapper class for FormulaError (to have null values).
    /// </summary>
    public class OperatorError
    {
        public OperatorError(FormulaError e)
        {
            error = e;
        }
        public FormulaError error { get; private set; }
    }
}
