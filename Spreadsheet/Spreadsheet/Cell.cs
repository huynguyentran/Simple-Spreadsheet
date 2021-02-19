using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{
    public class Cell
    {
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

        public Cell(object c)
        {
            if (ReferenceEquals(c, null))
                throw new ArgumentNullException("Cannot have a cell with a null value.");
            Contents = c;
        }


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
