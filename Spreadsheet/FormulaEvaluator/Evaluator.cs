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
        private readonly static Dictionary<Regex, Lookup> staticConverters = new Dictionary<Regex, Lookup>() {
            { new Regex(@"^\d+$") , int.Parse }};

        private readonly static Operator[] knownOperators = new Operator[] { new Plus(), new Minus(), new Times(), new Divide(), new LeftParenthesis(), new RightParenthesis() };

        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            Dictionary<Regex, Lookup> dynamicConverters = new Dictionary<Regex, Lookup>(staticConverters);
            dynamicConverters[new Regex(@"^[a-zA-Z]+\d+$")] = variableEvaluator;

            Stack<int> values = new Stack<int>();
            Stack<Operator> operators = new Stack<Operator>();

            foreach (string token in Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)"))
            {
                string tokenTrimmed = token.Trim();

                if (tokenTrimmed.Length > 0)
                {
                    if (TryParseValue(dynamicConverters, tokenTrimmed, out int valueRight))
                    {
                        if (operators.IsOnTop<Operator, Multiplicative>())
                        {
                            if (values.TryPop(out int valueLeft))
                                valueRight = operators.Pop().DoOperation(new int[] { valueLeft, valueRight });
                            else
                                throw new ArgumentException("Expected a value behind operator " + operators.Peek() + ", but got none.");
                        }

                        values.Push(valueRight);
                    }
                    else if (TryParseOperator(tokenTrimmed, out Operator objOperator))
                    {
                        objOperator.HandleStacks(values, operators);
                    }
                    else
                    {
                        throw new ArgumentException("The token " + tokenTrimmed + " was not recognized.");
                    }
                }
            }

            if (operators.Count == 1)
                Operator.DoOperationIf<Additive>(values, operators);

            if (values.Count == 1 && operators.Count == 0)
                return values.Pop();
            else
                throw new ArgumentException("Finished processing formula " + exp + " with excess operators or values. Is a left parenthesis missing a right pair?");
        }

        /// <summary>
        /// Determines whether a token is a value.
        /// </summary>
        /// <param name="values">A mapping of methods to identify values to methods that
        /// convert tokens into </param>
        /// <param name="token">The token to identify.</param>
        /// <param name="converter">The method to convert the token into an int.</param>
        /// <returns>Whether the token is a value.</returns>
        private static bool TryParseValue(Dictionary<Regex, Lookup> values, string token, out int conversion)
        {
            conversion = 0;

            foreach (KeyValuePair<Regex, Lookup> pair in values)
            {
                if (pair.Key.IsMatch(token))
                {
                    conversion = pair.Value(token);
                    return true;
                }
            }

            return false;
        }

        private static bool TryParseOperator(string token, out Operator type)
        {
            type = null;

            foreach(Operator op in knownOperators)
            {
                if (op.IsOperator(token))
                {
                    type = op;
                    return true;
                }
            }

            return false;
        }
    }
}
