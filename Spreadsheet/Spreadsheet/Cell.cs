using System;
using System.Collections.Generic;
using System.Text;

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

        public object Contents
        {
            get
            {
                return _contents;
            }

            private set
            {
                _contents = value;
            }
        }

        /// <summary>
        /// Creates a cell with non-null contents.
        /// </summary>
        /// <param name="c">The contents of the cell.</param>
        public Cell(object c)
        {
            if (ReferenceEquals(c, null))
                throw new ArgumentNullException("Cannot have a cell with a null value.");
            Contents = c;
        }

        /// <summary>
        /// Tries to cast the contents of the cell into the desired type.
        /// </summary>
        /// <typeparam name="T">The type to cast to.</typeparam>
        /// <param name="casted">The casted contents.</param>
        /// <returns>Whether the cast is possible.</returns>
        public bool CanCast<T>(out T casted)
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
