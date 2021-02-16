using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{
    public class Cell<T>
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

        public Cell(T c)
        {
            Contents = c;
        }

        public T GetCastContents()
        {
            return (T) Contents;
        }
    }
}
