using System;
using System.Collections.Generic;

namespace FormulaEvaluator
{
    /// <summary>
    /// A class that evaluates the value of functions.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// A function that searches for the value of a variable.
        /// </summary>
        /// <param name="v">The string representation of the variable.</param>
        /// <returns>The integer that the variable references.</returns>
        public delegate int Lookup(String v);

        /// <summary>
        /// The different kinds of values that can be found in a function.
        /// </summary>
        enum Value {Integer, Variable};

        /// <summary>
        /// A function that tells whether a token fits its type.
        /// </summary>
        /// <param name="token">The token to identify.</param>
        /// <returns>Whether the token is of the function's type.</returns>
        private delegate bool TokenIdentifier(String token);

        /// <summary>
        /// Mapping value types to functions.
        /// </summary>
        private readonly static Dictionary<Value, TokenIdentifier> identifiers = new Dictionary<Value, TokenIdentifier>() {
            { Value.Integer , IsInteger },
            { Value.Variable , IsVariable } };

        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
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
