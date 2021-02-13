using System;
using System.Collections.Generic;
using System.Text;


namespace SpreadsheetUtilities
{
    abstract class FormulaElement
    {
        protected abstract HashSet<Type> followers
        {
            get;
        }
        
        public bool CanFollow(FormulaElement next)
        {
            foreach (Type acceptable in followers)
                if (acceptable.IsInstanceOfType(next))
                    return true;
            return false;
        }
    }

    class Value : FormulaElement
    {
        protected override HashSet<Type> followers
        {
            get{ return new HashSet<Type>() { typeof(Additive), typeof(Multiplicative), typeof(RightParenthesis) };}
        }


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
