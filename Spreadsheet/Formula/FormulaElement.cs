using System;
using System.Collections.Generic;
using System.Text;


namespace SpreadsheetUtilities
{
    /// <summary>
    /// A basic element of a formula.
    /// </summary>
    abstract class FormulaElement
    {
        /// <summary>
        /// The types of formula elements that can follow this one.
        /// </summary>
        protected abstract HashSet<Type> followers
        {
            get;
        }
        
        /// <summary>
        /// Checks if another formula element can follow this one.
        /// </summary>
        /// <param name="next">The following formula element.</param>
        /// <returns>Whether the next token can follow this one.</returns>
        public bool CanFollow(FormulaElement next)
        {
            foreach (Type acceptable in followers)
                if (acceptable.IsInstanceOfType(next))
                    return true;
            return false;
        }
    }

    /// <summary>
    /// Represents any kind of formula value (double or variable).
    /// </summary>
    class Value : FormulaElement
    {
        /// <summary>
        /// Only additive, multiplicative, and right parenthesis operants
        /// </summary>
        protected override HashSet<Type> followers
        {
            get{ return new HashSet<Type>() { typeof(Additive), typeof(Multiplicative), typeof(RightParenthesis) };}
        }

        /// <summary>
        /// The string representation of the value.
        /// </summary>
        private string stringRep;

        public Value(string s)
        {
            stringRep = s;
        }

        public override string ToString()
        {
            return stringRep;
        }
    }
}
