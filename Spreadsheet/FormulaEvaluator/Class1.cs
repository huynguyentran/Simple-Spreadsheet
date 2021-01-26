using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// A class that evaluates the value of functions.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// A method that searches for the integer representing a value.
        /// </summary>
        /// <param name="v">The string representation of the value.</param>
        /// <returns>The integer that the value references.</returns>
        public delegate int Lookup(String v);

        /// <summary>
        /// The different kinds of values that can be found in a function.
        /// </summary>
        enum Value {Integer, Variable};

        /// <summary>
        /// A method that tells whether a token fits its type.
        /// </summary>
        /// <param name="token">The token to identify.</param>
        /// <returns>Whether the token is of the function's type.</returns>
        private delegate bool TokenIdentifier(String token);

        /// <summary>
        /// Mapping value types to identification methods.
        /// </summary>
        private readonly static Dictionary<Value, TokenIdentifier> identifiers = new Dictionary<Value, TokenIdentifier>() {
            { Value.Integer , IsInteger },
            { Value.Variable , IsVariable } };

        /// <summary>
        /// Mapping value types to conversion methods.
        /// </summary>
        private readonly static Dictionary<Value, Lookup> staticConverters = new Dictionary<Value, Lookup>() {
            { Value.Integer , int.Parse }};

        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            Dictionary<Value, Lookup> dynamicConverters = new Dictionary<Value, Lookup>(staticConverters);
            dynamicConverters.Add(Value.Variable, variableEvaluator);

            Stack<int> values = new Stack<int>();
            Stack<string> operators = new Stack<string>();

            foreach (string token in Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)"))
            {
                token.Trim();

                if (IsValue(token, out Value valueType))
                {
                    values.Push(dynamicConverters[valueType](token));
                }
                else
                {
                    //TODO: add operators to opertators stack.
                }

                //TODO: throw an error for an invalid token.
            }

            //TODO: evaluate the expressions using the stacks as outlined in the spec. 

            return 0;
        }

        /// <summary>
        /// Determines if a portion of a function token is an integer.
        /// </summary>
        /// <param name="sequence">The token.</param>
        /// <param name="startIndex">The index from which to start reading the token.</param>
        /// <returns>Whether the read portion of the token was an integer.</returns>
        private static bool IsInteger(String sequence, int startIndex)
        {
            if (startIndex > sequence.Length || startIndex < 0)
                throw new IndexOutOfRangeException("A number was asked to be identified from an index that did not exist within the length of the sequence to check.");
            else if (startIndex == sequence.Length)
                return false;

            while (startIndex < sequence.Length)
                if (!Char.IsNumber(sequence[startIndex++]))
                    return false;

            return true;
        }

        /// <summary>
        /// Determines if a token is an integer.
        /// </summary>
        /// <param name="sequence">The token.</param>
        /// <returns>Whether the token was an integer.</returns>
        private static bool IsInteger(String sequence)
        {
            return IsInteger(sequence, 0);
        }

        /// <summary>
        /// Determines if a token is a variable.
        /// </summary>
        /// <param name="sequence">The token.</param>
        /// <returns>Whether the token is a vairiable.</returns>
        private static bool IsVariable (String sequence)
        {
            if (sequence.Length < 2)
                return false;

            int i = 0;
            while (i < sequence.Length && Char.IsLetter(sequence[i++])) {}

            return i > 0 && IsInteger(sequence, i);
        }

        /// <summary>
        /// Determines whether a token is a value.
        /// </summary>
        /// <param name="token">The token to identify.</param>
        /// <param name="value">What kind of value the token is.</param>
        /// <returns>Whether the token is a value.</returns>
        private static bool IsValue(string token, out Value value)
        {
            value = Value.Integer;

            foreach (KeyValuePair<Value, TokenIdentifier> pair in identifiers)
            {
                if (pair.Value(token))
                {
                    value = pair.Key;
                    return true;
                }
            }

            return false;
        }
    }
}
