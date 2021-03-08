using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{
    /// <summary>
    /// A class that saves a spreadsheet to an arbitrary file type.
    /// </summary>
    /// <author>William Erignac</author>
    /// <version>2/25/2021</version>
    interface SpreadsheetFileManager
    {
        /// <summary>
        /// Saves a spreadsheet to a file.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet to save.</param>
        /// <param name="filename">Where to save the spreadsheet.</param>
        void Save(AbstractSpreadsheet spreadsheet, string filename);

        /// <summary>
        /// Fills a spreadsheet with the cells from a desired file.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet to fill.</param>
        /// <param name="filename">The location of the file.</param>
        void Load(AbstractSpreadsheet spreadsheet, string filename);

        /// <summary>
        /// Gets the version of a desired spreadsheet file.
        /// </summary>
        /// <param name="filename">The file to get the version of.</param>
        /// <returns>The string representation fo the version.</returns>
        string GetVersion(string filename); 
    }
}
