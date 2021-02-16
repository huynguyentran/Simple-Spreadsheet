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
            foreach (Type acceptable in followers) //Using IsInstanceOfType to consider inheritance,
                if (acceptable.IsInstanceOfType(next)) //as opposed to followers.Contains(next.GetType())
                    return true;
            return false;
        }

        /// <summary>
        /// Checks if this formula element can start a formula.
        /// </summary>
        /// <returns>Whether this formula element can start a formula.</returns>
        public virtual bool CanStart()
        {
            return true;
        }

        /// <summary>
        /// Checks if this formula element can end a formula.
        /// </summary>
        /// <returns>Whether this formula element can end a formula.</returns>
        public virtual bool CanEnd()
        {
            return true;
        }
    }

    /// <summary>
    /// Represents any kind of formula value (double or variable).
    /// </summary>
    class Value : FormulaElement
    {
        /// <summary>
        /// Only additive, multiplicative, and right parenthesis operands
        /// </summary>
        protected override HashSet<Type> followers
        {
            get{ return new HashSet<Type>() { typeof(Additive), typeof(Multiplicative), typeof(RightParenthesis) };}
        }

        /// <summary>
        /// The string representation of the value: either a re-stringified double, or a
        /// normalized variable name.
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
