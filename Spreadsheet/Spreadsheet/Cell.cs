using System;
using System.Collections.Generic;
using System.Text;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// Holds the contents of a single spreadsheet slot.
    /// </summary>
    /// <author>William Erignac</author>
    /// <version>2/19/2021</version>
    public class Cell
    {
        /// <summary>
        /// The literal item the cell contains (not necessarily what is printed).
        /// </summary>
        private object _contents;

        private object _value;

        private Func<string, double> lookup;

        public object Contents
        {
            get { return _contents; }

            private set { _contents = value; }
        }

        public object Value
        {
            get { return _value; }
            private set { _value = value; }
        }

        /// <summary>
        /// Creates a cell with non-null contents.
        /// </summary>
        /// <param name="c">The contents of the cell.</param>
        public Cell(Formula f, Func<string, double> l)
        {
            if (ReferenceEquals(f, null))
                throw new ArgumentNullException("Cannot have a cell with a null content.");
            Contents = f;
            if (ReferenceEquals(l, null))
                throw new ArgumentNullException("Cannot use a null lookup.");
            lookup = l; 
            UpdateValue();
        }

        /// <summary>
        /// Creates a cell with non-null contents.
        /// </summary>
        /// <param name="c">The contents of the cell.</param>
        public Cell(object c)
        {
            if (ReferenceEquals(c, null))
                throw new ArgumentNullException("Cannot have a cell with a null content.");
            Contents = c;
            Value = c;
        }

        /// <summary>
        /// Updates the value of a cell.
        /// </summary>
        /// <returns>Whether the cell was of a type that could be updated (i.e. Formula).</returns>
        public bool UpdateValue()
        {
            if (!ReferenceEquals(lookup, null))
            {
                Value = (Contents as Formula).Evaluate(lookup);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to cast the contents of the cell into the desired type.
        /// </summary>
        /// <typeparam name="T">The type to cast to.</typeparam>
        /// <param name="casted">The casted contents.</param>
        /// <returns>Whether the cast is possible.</returns>
        public bool TryCast<T>(out T casted)
        {
            casted = default(T);
            if (Contents is T t)
            {
                casted = t;
                return true;
            }
            return false;
        }
    }
}
