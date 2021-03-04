using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SpreadsheetUtilities;
using System.IO;

namespace SS
{
    /// <summary>
    /// A class that converts xml files to spreadsheets and back.
    /// </summary>
    /// <author>William Erignac</author>
    /// <version>2/26/2021</version>
    class SpreadsheetXMLManager : SpreadsheetFileManager
    {
        /// <summary>
        /// The name used to signify a spreadsheet element.
        /// </summary>
        private const string spreadsheetKey = "spreadsheet";
        /// <summary>
        /// The name used to signify the version attribute of a spreadsheet.
        /// </summary>
        private const string versionKey = "version";
        /// <summary>
        /// The name used to signify a cell element.
        /// </summary>
        private const string cellKey = "cell";
        /// <summary>
        /// The name used to signify a cell name element.
        /// </summary>
        private const string nameKey = "name";
        /// <summary>
        /// The name used to signify a cell contents element.
        /// </summary>
        private const string contentKey = "contents";

        /// <summary>
        /// Gets an xml reader or writer for a file while ensuring the file exists.
        /// </summary>
        /// <typeparam name="T">An xml reader or writer.</typeparam>
        /// <param name="filepath">The file to open.</param>
        /// <param name="getter">A function that opens the reader or writer.</param>
        /// <returns>The reader or writer.</returns>
        private T GetReaderOrWiter<T>(string filepath, Func<string, T> getter)
        {
            try
            {
                return getter(filepath);
            }//If there's an exception, we repackage it into a SpreadsheetReadWriteException.
            catch(FileNotFoundException e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
            catch(DirectoryNotFoundException e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// Gets the version of an xml spreadsheet file.
        /// </summary>
        /// <param name="filepath">The file to read.</param>
        /// <returns>The version of the spreadsheet file.</returns>
        public string GetVersion(string filepath)
        {
            using (XmlReader reader = GetReaderOrWiter(filepath, path => XmlReader.Create(path)))
            {
                //The version should be the first attribute of the first element (spreadsheet).
                if (reader.Read() && reader.IsStartElement())
                    if (reader.Name == spreadsheetKey && !ReferenceEquals(reader[versionKey], null))
                        return reader[versionKey];
            }
            throw new SpreadsheetReadWriteException("Was unable to find the spreadsheet's version (was not at the top of the file).");
        }

        /// <summary>
        /// Fills a spreadsheet with the cells found in an xml spreadsheet file.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet to fill.</param>
        /// <param name="filepath">The file to fill the spreadsheet with.</param>
        public void Load(AbstractSpreadsheet spreadsheet, string filepath)
        {
            using (XmlReader reader = GetReaderOrWiter(filepath, path => XmlReader.Create(path)))
            {
                //Whether we're inside a spreadsheet element.
                bool inSpreadsheet = false;
                //The name of the current cell being added.
                string currentCellName = null;
                //The contents of the current cell being added.
                string currentCellContents = null;

                //Read the entire file...
                while(CanRead(reader))
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case spreadsheetKey:
                                //We've entered a spreadsheet element.
                                inSpreadsheet = true;
                                if (reader[versionKey] != spreadsheet.Version)
                                    throw new SpreadsheetReadWriteException("Incompatible versions " + reader[versionKey] + " (old) and " + spreadsheet.Version + " (new).");
                                break;
                            case cellKey:
                                //We've entered a cell element.
                                if (!inSpreadsheet)
                                    throw new SpreadsheetReadWriteException("Found a lone cell without a spreadsheet.");
                                break;
                            case nameKey:
                                //We've entered a cell name element.
                                if (! ReferenceEquals(currentCellName, null))
                                    throw new SpreadsheetReadWriteException("Found two names for the same cell.");
                                reader.Read();
                                currentCellName = reader.Value;
                                break;
                            case contentKey:
                                //We've entered a cell content element.
                                if (!ReferenceEquals(currentCellContents, null))
                                    throw new SpreadsheetReadWriteException("Found two contents for the same cell.");
                                reader.Read();
                                currentCellContents = reader.Value;
                                break;
                        }
                    }
                    else //If we aren't at the start of an element, we're at the end.
                    {
                        if (reader.Name == cellKey)
                        {
                            //When we hit the end of a cell block, we can make and add the cell.
                            if (ReferenceEquals(currentCellName, null))
                                throw new SpreadsheetReadWriteException("Wasn't able to find a cell's name before the cell closed.");
                            if (ReferenceEquals(currentCellContents, null))
                                throw new SpreadsheetReadWriteException("Wasn't able to find a cell's contents before the cell closed.");

                            try
                            {
                                spreadsheet.SetContentsOfCell(currentCellName, currentCellContents);
                            }
                            catch(CircularException c)
                            {
                                throw new SpreadsheetReadWriteException("A circular exception was encountered while loading a spreadsheet.");
                            }
                            catch(ArgumentNullException a)
                            {
                                throw new SpreadsheetReadWriteException(a.Message);
                            }
                            catch(InvalidNameException i)
                            {
                                throw new SpreadsheetReadWriteException(i.Message);
                            }

                            //We set these back to null to prepare for the next cell.
                            currentCellName = null;
                            currentCellContents = null;
                        }
                        else if (reader.Name == spreadsheetKey)
                        {
                            //We should only exit the spreadsheet once at hte end of the file.
                            inSpreadsheet = false;
                        }
                    }
                }
            }
        }

        private bool CanRead(XmlReader reader)
        {
            try
            {
                return reader.Read();
            }
            catch(XmlException e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// Saves a spreadsheet as an xml file.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet to save.</param>
        /// <param name="filepath">Where to save the spreadsheet.</param>
        public void Save(AbstractSpreadsheet spreadsheet, string filepath)
        {
            //Make sure we have human-friendly formatting!
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            using (XmlWriter writer = GetReaderOrWiter(filepath, path => XmlWriter.Create(path, settings)))
            {
                //Every file must start with a spreadsheet.
                writer.WriteStartDocument();
                writer.WriteStartElement(spreadsheetKey);
                writer.WriteAttributeString(versionKey, spreadsheet.Version);

                //Adding each cell in the spreadsheet to the file.
                foreach(string cellName in spreadsheet.GetNamesOfAllNonemptyCells())
                {
                    writer.WriteStartElement(cellKey);

                    //Name
                    writer.WriteElementString(nameKey, cellName);
                    //Contents
                    object contents = spreadsheet.GetCellContents(cellName);
                    if (contents is Formula f)
                        writer.WriteElementString(contentKey, "=" + f.ToString());
                    else
                        writer.WriteElementString(contentKey, contents.ToString());

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
