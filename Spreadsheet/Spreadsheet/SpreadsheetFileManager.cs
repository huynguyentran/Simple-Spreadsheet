using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{
    interface SpreadsheetFileManager
    {
        void Save(AbstractSpreadsheet spreadsheet, string filename);

        void Load(AbstractSpreadsheet spreadsheet, string filename);

        string GetVersion(string filename); 
    }
}
