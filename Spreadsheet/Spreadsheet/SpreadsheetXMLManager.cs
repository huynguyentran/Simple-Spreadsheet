using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SpreadsheetUtilities;
using System.IO;

namespace SS
{
    class SpreadsheetXMLManager : SpreadsheetFileManager
    {
        private const string versionKey = "version";

        private const string spreadsheetKey = "spreadsheet";

        private const string cellKey = "cell";

        private const string nameKey = "name";

        private const string contentKey = "contents";

        public string GetVersion(string filepath)
        {
            using (XmlReader reader = XmlReader.Create(filepath))
            {
                if (reader.Read() && reader.IsStartElement())
                    if (reader.Name == spreadsheetKey)
                        return reader[versionKey];
            }
            throw new SpreadsheetReadWriteException("Was unable to find the spreadsheet's version (was not at the top of the file or file didn't exist).");
        }

        public void Load(AbstractSpreadsheet spreadsheet, string filepath)
        {
            using (XmlReader reader = XmlReader.Create(filepath))
            {
                string currentCellName = null;
                string currentCellContents = null;

                while(reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case spreadsheetKey:
                                break;
                            case cellKey:
                                break;
                            case nameKey:
                                reader.Read();
                                currentCellName = reader.Value;
                                break;
                            case contentKey:
                                reader.Read();
                                currentCellContents = reader.Value;
                                break;
                        }
                    }
                    else
                    {
                        if (reader.Name == cellKey)
                        {
                            if (ReferenceEquals(currentCellName, null))
                                throw new SpreadsheetReadWriteException("Wasn't able to find a cell's name before the cell closed.");
                            if (ReferenceEquals(currentCellContents, null))
                                throw new SpreadsheetReadWriteException("Wasn't able to find a cell's contents before the cell closed.");

                            spreadsheet.SetContentsOfCell(currentCellName, currentCellContents);

                            currentCellName = null;
                            currentCellContents = null;
                        }
                    }
                }
            }
        }

        public void Save(AbstractSpreadsheet spreadsheet, string filepath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            using (XmlWriter writer = XmlWriter.Create(filepath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(spreadsheetKey);
                writer.WriteAttributeString(versionKey, spreadsheet.Version);

                foreach(string cellName in spreadsheet.GetNamesOfAllNonemptyCells())
                {
                    object contents = spreadsheet.GetCellContents(cellName);

                    writer.WriteStartElement(cellKey);

                    writer.WriteElementString(nameKey, cellName);

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
