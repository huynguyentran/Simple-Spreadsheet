using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// A method that searches for the integer representing a value.
    /// </summary>
    /// <param name="v">The string representation of the value.</param>
    /// <returns>The integer that the value references.</returns>
    public delegate int Lookup(String v);

    /// <summary>
    /// A method that tells whether a token fits its type.
    /// </summary>
    /// <param name="token">The token to identify.</param>
    /// <returns>Whether the token is of the function's type.</returns>
    public delegate bool TokenIdentifier(String token);

    /// <summary>
    /// A class that evaluates the value of functions.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Mapping value identifiers to conversion methods (when both are known).
        /// </summary>
        private readonly static Dictionary<TokenIdentifier, Lookup> staticConverters = new Dictionary<TokenIdentifier, Lookup>() {
            { IsInteger , int.Parse }};

        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            Dictionary<TokenIdentifier, Lookup> dynamicConverters = new Dictionary<TokenIdentifier, Lookup>(staticConverters);
            dynamicConverters[IsVariable] = variableEvaluator;

            Stack<int> values = new Stack<int>();
            Stack<string> operators = new Stack<string>();

            foreach (string token in Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)"))
            {
                string tokenTrimmed = token.Trim();

                if (TryParseValue(dynamicConverters, tokenTrimmed, out int intValue))
                {
                    if (operators.IsOnTop("*") || operators.IsOnTop("/"))
                    {
                        if (values.Count == 0)
                            throw new ArgumentException("Expected a value behind operator " + operators.Peek() + ", but got none.");
                        intValue = DoOperations(values.Pop(), intValue, operators.Pop());
                    }

                    values.Push(intValue);
                }
                else if (IsOperator(tokenTrimmed))
                {
                    if (tokenTrimmed.Equals("*") || tokenTrimmed.Equals("/"))
                    {
                        operators.Push(tokenTrimmed);
                    }
                    else if (tokenTrimmed.Equals("+") || tokenTrimmed.Equals("-"))
                    {
                        operators.Push(tokenTrimmed);
                    }
                    else if (tokenTrimmed.Equals("("))
                    {
                        operators.Push(tokenTrimmed);
                    }
                    else if (tokenTrimmed.Equals(")"))
                    {
                        
                    }
                }
                else
                {
                    throw new ArgumentException("The token " + tokenTrimmed + " was not recognized.");
                }
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
            i--;
            return i > 0 && IsInteger(sequence, i);
        }

        /// <summary>
        /// Determines whether a token is a value.
        /// </summary>
        /// <param name="values">A mapping of methods to identify values to methods that
        /// convert tokens into </param>
        /// <param name="token">The token to identify.</param>
        /// <param name="converter">The method to convert the token into an int.</param>
        /// <returns>Whether the token is a value.</returns>
        private static bool TryParseValue(Dictionary<TokenIdentifier, Lookup> values, string token, out int conversion)
        {
            conversion = 0;

            foreach (KeyValuePair<TokenIdentifier, Lookup> pair in values)
            {
                if (pair.Key(token))
                {
                    conversion = pair.Value(token);
                    return true;
                }
            }

            return false;
        }
    }
}
