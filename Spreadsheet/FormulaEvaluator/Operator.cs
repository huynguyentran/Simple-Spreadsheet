using System;
using System.Collections.Generic;

namespace FormulaEvaluator
{
    /// <summary>
    /// The abstract representation of an integer operator.
    /// </summary>
    abstract class Operator
    {
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
        /// <returns>The integer output of the operation.</returns>
        public virtual int DoOperation(int[] operands)
        {
            if (operands.Length != GetOperandCount())
                throw new ArgumentException("Operator " + this + " takes " + GetOperandCount() + " operands, but got " + operands.Length + ".");
            return 1;
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
        /// <param name="values">The integer values processed thus far by the FunctionEvaluator.</param>
        /// <param name="operators">The operators processed thus far by the FunctionEvaluator.</param>
        public virtual void HandleStacks(Stack<int> values, Stack<Operator> operators)
        {
            operators.Push(this);
        }

        /// <summary>
        /// Performs an operator's function on the most recent values extracted by the FunctionEvaluator.
        /// </summary>
        /// <param name="values">The integer values processed thus far by the FunctionEvaluator.</param>
        /// <param name="op">The operator to evaluate with the values's operands.</param>
        public static void DoOperationWith(Stack<int> values, Operator op)
        {
            if (values.TryPops(op.GetOperandCount(), out int[] operands))
                values.Push(op.DoOperation(operands));
            else
                throw new ArgumentException("Tried to perform operation, but operands were missing.");
        }

        /// <summary>
        /// Performs an operation on the most recent values extracted by the FunctionEvaluator 
        /// <para>if the most recent operator extracted FunctionEvaluator by matches the desired operation.
        /// </summary>
        /// <typeparam name="T">The type of operator to look for.</typeparam>
        /// <param name="values">The integer values processed thus far by the FunctionEvaluator.</param>
        /// <param name="operators">The operators processed thus far by the FunctionEvaluator.</param>
        public static void DoOperationIf<T>(Stack<int> values, Stack<Operator> operators) where T: Operator
        {
            if (operators.IsOnTop<Operator, T>())
                DoOperationWith(values, operators.Pop());
        }
    }
}
