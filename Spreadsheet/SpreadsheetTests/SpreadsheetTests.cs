using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace SpreadsheetTests
{
    /// <author>William Erignac</author>
    /// <version>2/24/2021</version>
    [TestClass]
    public class SpreadsheetTests
    {
        private readonly Func<string, bool> rowColFormat = s => Regex.IsMatch(s, @"^[A-Z]+[0-9]+$");
        private readonly Func<string, string> upperCase = s => s.ToUpper();

        [TestMethod]
        public void Empty()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("arbitrary"));
            IEnumerator<string> e = s.GetNamesOfAllNonemptyCells().GetEnumerator();
            Assert.IsFalse(e.MoveNext());
            
            Assert.IsTrue(s.IsValid("= This doesn't aCtually chECk anything... )*+"));
            string contents = "noTHing c4n haPpen t0 m3";
            Assert.AreEqual(contents, s.Normalize(contents));
            Assert.AreEqual("default", s.Version);
            Assert.IsFalse(s.Changed);
        }

        [TestMethod]
        public void ThreeParameterConstructor()
        {
            AbstractSpreadsheet s = new Spreadsheet(rowColFormat, upperCase, "relevant");
            Assert.AreEqual("", s.GetCellContents("arbitrary8"));
            IEnumerator<string> e = s.GetNamesOfAllNonemptyCells().GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            Assert.IsFalse(s.IsValid("thisTimeItChecks"));
            Assert.IsTrue(s.IsValid("ARC9897"));
            string contents = "something will happen";
            Assert.AreEqual("SOMETHING WILL HAPPEN", s.Normalize(contents));
            Assert.AreEqual("relevant", s.Version);
            Assert.IsFalse(s.Changed);
        }

        private void CheckCell(AbstractSpreadsheet s, string cell, object expectedContents, object expectedValue)
        {
            if (expectedContents is double dC)
                Assert.AreEqual(dC, (double)s.GetCellContents(cell), 1e-9);
            else
                Assert.AreEqual(expectedContents, s.GetCellContents(cell));

            if (expectedValue is double dV)
                Assert.AreEqual(dV, (double)s.GetCellValue(cell), 1e-9);
            else if (expectedValue is FormulaError f)
                Assert.IsTrue(s.GetCellValue(cell) is FormulaError);
            else
                Assert.AreEqual(expectedValue, s.GetCellValue(cell));
        }

        private void CheckCell(AbstractSpreadsheet s, string cell, object expectedContents)
        {
            CheckCell(s, cell, expectedContents, expectedContents);
        }

        [TestMethod]
        public void AddingCells()
        {
            AbstractSpreadsheet s = new Spreadsheet();

            string food = "Sandvich";
            string cell = "a1";
            s.SetContentsOfCell(cell, food);
            CheckCell(s, cell, food);

            double number = 2.8d;
            cell = "b2";
            s.SetContentsOfCell(cell, number.ToString());
            CheckCell(s, cell, number);

            string formStr = "=b2 + 0.3";
            cell = "c3";
            s.SetContentsOfCell(cell, formStr);
            CheckCell(s, cell, new Formula(formStr.Substring(1)), 3.1d);

            string noVarStr = "=burrito9 - 2";
            cell = "d4";
            s.SetContentsOfCell(cell, noVarStr);
            CheckCell(s, cell, new Formula(noVarStr.Substring(1)), new FormulaError());

            string zeroDiv = "=25 / (1 - 3 / 3)";
            cell = "e5";
            s.SetContentsOfCell(cell, zeroDiv);
            CheckCell(s, cell, new Formula(zeroDiv.Substring(1)), new FormulaError());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidCellName()
        {
            AbstractSpreadsheet s = new Spreadsheet(rowColFormat, upperCase, "irrelevant");
            s.SetContentsOfCell("Portal", "the cake is a lie");
        }
        
        [TestMethod]
        public void NormalizerWorks()
        {
            AbstractSpreadsheet s = new Spreadsheet(rowColFormat, upperCase, "irrelevant");
            s.SetContentsOfCell("a1", 3d.ToString());
            CheckCell(s, "A1", 3d);

            string formStr = "=a1*2";

            s.SetContentsOfCell("b2", formStr);
            CheckCell(s, "B2", new Formula(formStr.Substring(1)), 6d);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullNameDouble()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, 15.32d.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullNameString()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "Guac");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullNameFormula()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "=3 - 2 + (9 * 8)");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void BadNameDouble()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("", 15.32d.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void BadNameString()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("2x", "Guac");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void BadNameFormula()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("_&", "=3 - 2 + (9 * 8)");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullString()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            string value = null;
            s.SetContentsOfCell("Oh_this_is_bad", value);
        }

        
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SmallestCycle()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A1");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void LargeCycle()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B2 + 9");
            s.SetContentsOfCell("B2", "=C3 - 2");
            s.SetContentsOfCell("C3", "=E5/D4*17.5");
            s.SetContentsOfCell("E5", "=F7*(81/(3+F7))");
            s.SetContentsOfCell("D4", 0.332.ToString());
            s.SetContentsOfCell("G8", "=A1 - 90");
            s.SetContentsOfCell("H9", "how is this a string?");
            s.SetContentsOfCell("F7", "=G8 * (H9-1)/3");
        }

        private AbstractSpreadsheet MakeSpreadsheet(Dictionary<string, object> cells, Func<string, bool> validator, Func<string, string> normalizer, string version)
        {
            AbstractSpreadsheet s = new Spreadsheet(validator, normalizer, version);

            foreach(KeyValuePair<string, object> pair in cells)
            {
                if (pair.Value is double d)
                    s.SetContentsOfCell(pair.Key, d.ToString());
                else if (pair.Value is string str)
                    s.SetContentsOfCell(pair.Key, str);
                else if (pair.Value is Formula f)
                    s.SetContentsOfCell(pair.Key, "=" + f.ToString());
                else
                    throw new ArgumentException("Did not recognize value " + pair.Value + " of type " + pair.Value.GetType() + " of cell " + pair.Key +".");
            }

            return s;
        }

        private AbstractSpreadsheet MakeSpreadsheet(Dictionary<string,object> cells)
        {
            return MakeSpreadsheet(cells, s => true, s => s, "default");
        }

        private AbstractSpreadsheet BasicContentsTest(Dictionary<string, object> d, AbstractSpreadsheet s)
        {
            d = new Dictionary<string, object>(d);

            IEnumerable<string> cellNames = s.GetNamesOfAllNonemptyCells();

            foreach(string cell in cellNames)
            {
                object contents = s.GetCellContents(cell);

                if (contents is double doub)
                    Assert.AreEqual((double)d[cell], doub);
                else if (contents is string str)
                    Assert.AreEqual((string)d[cell], str);
                else if (contents is Formula f)
                    Assert.AreEqual((Formula)d[cell], f);
                else
                    throw new ArgumentException("Unexpected cell " + cell + " with contents " + contents + "of type " + contents.GetType() + ".");

                d.Remove(cell);
            }

            if (d.Count != 0)
                Assert.Fail("A spreadsheet is missing " + d.Count + "cells.");

            return s;
        }

        private AbstractSpreadsheet BasicContentsTest(Dictionary<string, object> d)
        {
            return BasicContentsTest(d, MakeSpreadsheet(d));
        }

        [TestMethod]
        public void NonFormulaContents()
        {
            Dictionary<string, object> d = new Dictionary<string, object>();
            d.Add("hamburgler", "stolen burgers");
            d.Add("pi", 3.1415);
            d.Add("e", 2.7);
            d.Add("i", "sqrt(-1)");
            d.Add("circumference", new Formula("2*pi*radius"));
            d.Add("area", new Formula("circumference*circumference/(4*pi)"));

            AbstractSpreadsheet s = BasicContentsTest(d);

            Assert.AreEqual("", s.GetCellContents("thi5_1s_n0t_4_r3gisered_ce1l"));
        }

        private void CheckRecalculateList(HashSet<string>[] expected, IList<string> output)
        {
            int i = 0;

            foreach(string dependence in output)
            {
                if (!expected[i].Remove(dependence))
                    Assert.Fail("Unexpected dependence " + dependence + " at level " + i + ".");

                while (i < expected.Length && expected[i].Count == 0) { i++; }
            }

            if (!(i == expected.Length && expected[expected.Length - 1].Count == 0))
                Assert.Fail("Not all dependencies were found.");
        }

        private void CheckRecalculateList(string expected, IList<string> output)
        {
            Assert.AreEqual(1, output.Count);
            Assert.AreEqual(expected, output[0]);
        }

        [TestMethod]
        public void CorrectCellsToRecalculate()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            IList<string> list =  s.SetContentsOfCell("a1", 43.ToString());
            CheckRecalculateList("a1", list);

            list = s.SetContentsOfCell("b2", "=a1 + 3");
            CheckRecalculateList("b2", list);

            list = s.SetContentsOfCell("c3", "=a1 - 2");
            CheckRecalculateList("c3", list);

            list = s.SetContentsOfCell("a1", 49.ToString());
            Assert.AreEqual(3, list.Count);
            CheckRecalculateList(new HashSet<string>[] { new HashSet<string>() {"a1"}, new HashSet<string>() { "b2", "c3" } }, list);

            list = s.SetContentsOfCell("d4", "=b2 - c3");
            CheckRecalculateList("d4", list);

            HashSet<string>[] nextDependents = new HashSet<string>[] { new HashSet<string>() {"a1"}, 
                new HashSet<string>() { "b2","c3" },
                new HashSet<string>() {"d4"}};

            list = s.SetContentsOfCell("a1", "=c9 - goober");
            CheckRecalculateList(nextDependents, list);

            nextDependents = new HashSet<string>[] { new HashSet<string>() {"b2"},
                new HashSet<string>() {"d4"}};

            list = s.SetContentsOfCell("b2", "=a1 * c3");
            CheckRecalculateList(nextDependents, list);

            nextDependents = new HashSet<string>[] { new HashSet<string>() {"c3"},
                new HashSet<string>() {"d4", "b2"}};

            list = s.SetContentsOfCell("c3", "=goober - c9");
            CheckRecalculateList(nextDependents, list);

            nextDependents = new HashSet<string>[] { new HashSet<string>() {"a1"},
                new HashSet<string>() {"b2"},
                new HashSet<string>() {"d4"}};

            list = s.SetContentsOfCell("a1", "this is the end of the test");
            CheckRecalculateList(nextDependents, list);
        }

        [TestMethod]
        public void SpreadsheetKeepsFormAfterCircularException()
        {
            Dictionary<string, object> cells = new Dictionary<string, object>();
            cells.Add("a1", new Formula("a2 + a9"));
            cells.Add("a2", new Formula("b1 - 3"));
            cells.Add("a9", new Formula("b2 * b1"));
            AbstractSpreadsheet s = MakeSpreadsheet(cells);
            bool noError = true;

            HashSet<string>[] nextDependents = new HashSet<string>[] { new HashSet<string>() {"b2"},
                new HashSet<string>() { "a9" },
                new HashSet<string>() {"a1"}};

            try
            {
                s.SetContentsOfCell("b1", "=a1");
            }
            catch(CircularException c)
            {
                noError = false;
                CheckRecalculateList("a1", s.SetContentsOfCell("a1", "=a2 + a9"));
                CheckRecalculateList(nextDependents, s.SetContentsOfCell("b2", 3.ToString()));

                Assert.IsFalse(s.GetNamesOfAllNonemptyCells().Contains("b1"));
            }

            if (noError)
                Assert.Fail("Expected a CircularException, got none.");
        }

        [TestMethod]
        public void AddingEmptyCell()
        {
            Dictionary<string, object> cells = new Dictionary<string, object>();
            cells.Add("a1", new Formula("c3 + b2"));
            cells.Add("c3", new Formula("d4 * 2"));
            cells.Add("b2", new Formula("e5 * 2"));
            cells.Add("e5", new Formula("d4 * variable"));

            AbstractSpreadsheet s = MakeSpreadsheet(cells);

            HashSet<string>[] nextDependents = new HashSet<string>[]
            {
                new HashSet<string>() {"d4"},
                new HashSet<string>() {"e5", "c3","a1", "b2"}
            };

            CheckRecalculateList(nextDependents, s.SetContentsOfCell("d4", 3.ToString()));

            nextDependents = new HashSet<string>[]
            {
                new HashSet<string>() {"b2"},
                new HashSet<string>() {"a1"}
            };

            CheckRecalculateList(nextDependents, s.SetContentsOfCell("b2", ""));
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().Contains("b2"));

            CheckRecalculateList("e5", s.SetContentsOfCell("e5", ""));
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().Contains("e5"));
        }

        private void CheckSavedSpreadsheet(string location, string version, Dictionary<string, object> cells)
        {
            using (XmlReader reader = XmlReader.Create(location))
            {
                bool inSheet = false;
                bool inCell = false;

                string currentCellName = null;
                string currentCellContents = null;

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                Assert.AreEqual(version, reader["version"]);
                                if (!inSheet)
                                    inSheet = true;
                                else
                                    Assert.Fail("Found spreadsheet inside of a spreadsheet which should not occur.");
                                break;
                            case "cell":
                                if (!inSheet)
                                    Assert.Fail("Cannot be in cell without being in spreadsheet.");
                                if (!inCell)
                                    inCell = true;
                                else
                                    Assert.Fail("Found cell inside of cell, which should not occur.");
                                break;
                            case "name":
                                if (!inCell)
                                    Assert.Fail("Name cannot exist outside of cell.");
                                reader.Read();
                                currentCellName = reader.Value;
                                break;
                            case "contents":
                                if (!inCell)
                                    Assert.Fail("Contents cannot exist outside of cell.");
                                reader.Read();
                                currentCellContents = reader.Value;
                                break;
                        }
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "cell":
                                if (ReferenceEquals(currentCellName, null))
                                    Assert.Fail("Wasn't able to find a cell's name before the cell closed.");
                                if (ReferenceEquals(currentCellContents, null))
                                    Assert.Fail("Wasn't able to find a cell's contents before the cell closed.");

                                Assert.AreEqual(currentCellContents, cells[currentCellName]);
                                Assert.IsTrue(cells.Remove(currentCellName));

                                inCell = false;
                                currentCellName = null;
                                currentCellContents = null;
                                break;

                            case "spreadsheet":
                                inSheet = false;
                                break;
                        }
                    }
                }

                Assert.IsFalse(inSheet);
                Assert.IsFalse(inCell);
            }

            Assert.AreEqual(0, cells.Count);
        }

        private void CheckSavedSpreadsheet(string location, Dictionary<string, object> cells)
        {
            CheckSavedSpreadsheet(location, "default", cells);
        }

        [TestMethod]
        public void SavingSimpleSpreadsheet()
        {
            Dictionary<string, object> cells = new Dictionary<string, object>();
            cells.Add("a1", "31");
            cells.Add("b1", "Torbjorn");
            cells.Add("c1", "=a1*2");

            AbstractSpreadsheet s = MakeSpreadsheet(cells);

            string location = "writeSimpleSpreadsheet.xml";

            Assert.IsTrue(s.Changed);
            s.Save(location);
            Assert.IsFalse(s.Changed);

            CheckSavedSpreadsheet(location, cells);
        }

        [TestMethod]
        public void SavingSpreadsheetWithNormalizer()
        {
            Dictionary<string, object> cells = new Dictionary<string, object>();
            cells.Add("robot", "chicken");
            cells.Add("family", "guy");
            cells.Add("spongebob", "squarepants");

            AbstractSpreadsheet s = MakeSpreadsheet(cells, s=> true, upperCase, "default");

            Dictionary<string, object> upperCaseCells = new Dictionary<string, object>();

            foreach(KeyValuePair<string, object> pair in cells)
            {
                upperCaseCells.Add(upperCase(pair.Key), pair.Value);
            }

            string location = "writeNormalizerSpreadsheet.xml";

            s.Save(location);

            CheckSavedSpreadsheet(location, upperCaseCells);
        }

        private void WriteSpreadsheet(string location, string version, Dictionary<string, object> cells)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            using (XmlWriter writer = XmlWriter.Create(location))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", version);

                foreach(KeyValuePair<string, object> pair in cells)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", pair.Key);
                    if (pair.Value is string s)
                        writer.WriteElementString("contents", s);
                    else if (pair.Value is double d)
                        writer.WriteElementString("contents", d.ToString());
                    else if (pair.Value is Formula f)
                        writer.WriteElementString("contents", "=" + f.ToString());

                    writer.WriteEndElement();
                }


                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        [TestMethod]
        public void LoadSimpleSpreadsheet()
        {
            Dictionary<string, object> cells = new Dictionary<string, object>();
            cells.Add("a1", 31d);
            cells.Add("b1", "Torbjorn");
            cells.Add("c1", new Formula("a1*2"));

            string location = "readSimpleSpreadsheet.xml";

            WriteSpreadsheet(location, "default", cells);

            AbstractSpreadsheet s = BasicContentsTest(cells, new Spreadsheet(location, s => true, s => s, "good version"));

            Assert.IsFalse(s.Changed);
            Assert.AreEqual("good version", s.Version);
        }

        [TestMethod]
        public void LoadEmptySpreadsheet()
        {
            Dictionary<string, object> cells = new Dictionary<string, object>();

            string location = "readEmptySpreadsheet.xml";

            WriteSpreadsheet(location, "default", cells);

            AbstractSpreadsheet s = new Spreadsheet(location, s => true, s=> s, "default");

            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void LoadNonExistentDirectory()
        {
            new Spreadsheet("/this_location_probably_doesnt_exist_but_if_it_did_that_would_be_weird/teehsdaerps.xml",
                s => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void LoadNonExistentFile()
        {
            new Spreadsheet("shrodeingersSpreadsheetItExistsAndDoesntUntilObserved.xml",
                s => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void LoadEmptyFile()
        {
            string location = "empty.xml";
            using(XmlWriter writer = XmlWriter.Create(location))
            {
                writer.WriteStartDocument();
                writer.WriteEndDocument();
            }

            new Spreadsheet(location, s => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void LoadBrokenSpreadsheetFile()
        {
            string location = "badSpreadsheet.xml";
            using (XmlWriter writer = XmlWriter.Create(location))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "roger");
                writer.WriteElementString("contents", "roger");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "oops");
                writer.WriteElementString("contents", "this is no good");
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            new Spreadsheet(location, s => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void LoadBrokenCellFile()
        {
            string location = "badCell.xml";
            using (XmlWriter writer = XmlWriter.Create(location))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "roger");
                writer.WriteEndElement();
                writer.WriteElementString("contents", "oh this is bad");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            new Spreadsheet(location, s => true, s => s, "default");
        }

        [TestMethod]
        public void FindSpreadsheetVersion()
        {
            WriteSpreadsheet("coolSheet.xml", "cool", new Dictionary<string, object>());
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("cool", s.GetSavedVersion("coolSheet.xml"));
        }
    }
}
