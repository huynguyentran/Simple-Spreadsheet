using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace SpreadsheetTests
{
    /// <author>William Erignac</author>
    /// <version>2/19/2021</version>
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
            Assert.AreEqual("", s.GetCellContents("arbitrary"));
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

        /*
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SmallestCycle()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void LargeCycle()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("B2 + 9"));
            s.SetCellContents("B2", new Formula("C3 - 2"));
            s.SetCellContents("C3", new Formula("E5/D4*17.5"));
            s.SetCellContents("E5", new Formula("F7*(81/(3+F7))"));
            s.SetCellContents("D4", 0.332);
            s.SetCellContents("G8", new Formula("A1 - 90"));
            s.SetCellContents("H9", "how is this a string?");
            s.SetCellContents("F7", new Formula("G8 * (H9-1)/3"));
        }

        private AbstractSpreadsheet MakeSpreadsheet(Dictionary<string, object> cells)
        {
            AbstractSpreadsheet s = new Spreadsheet();

            foreach(KeyValuePair<string, object> pair in cells)
            {
                if (pair.Value is double d)
                    s.SetCellContents(pair.Key, d);
                else if (pair.Value is string str)
                    s.SetCellContents(pair.Key, str);
                else if (pair.Value is Formula f)
                    s.SetCellContents(pair.Key, f);
                else
                    throw new ArgumentException("Did not recognize value " + pair.Value + " of type " + pair.Value.GetType() + " of cell " + pair.Key +".");
            }

            return s;
        }

        [TestMethod]
        public AbstractSpreadsheet BasicContentsTest(Dictionary<string, object> d)
        {
            d = new Dictionary<string, object>(d);

            AbstractSpreadsheet s = MakeSpreadsheet(d);

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
            IList<string> list =  s.SetCellContents("a1", 43);
            CheckRecalculateList("a1", list);

            list = s.SetCellContents("b2", new Formula("a1 + 3"));
            CheckRecalculateList("b2", list);

            list = s.SetCellContents("c3", new Formula("a1 - 2"));
            CheckRecalculateList("c3", list);

            list = s.SetCellContents("a1", 49);
            Assert.AreEqual(3, list.Count);
            CheckRecalculateList(new HashSet<string>[] { new HashSet<string>() {"a1"}, new HashSet<string>() { "b2", "c3" } }, list);

            list = s.SetCellContents("d4", new Formula("b2 - c3"));
            CheckRecalculateList("d4", list);

            HashSet<string>[] nextDependents = new HashSet<string>[] { new HashSet<string>() {"a1"}, 
                new HashSet<string>() { "b2","c3" },
                new HashSet<string>() {"d4"}};

            list = s.SetCellContents("a1", new Formula("c9 - goober"));
            CheckRecalculateList(nextDependents, list);

            nextDependents = new HashSet<string>[] { new HashSet<string>() {"b2"},
                new HashSet<string>() {"d4"}};

            list = s.SetCellContents("b2", new Formula("a1 * c3"));
            CheckRecalculateList(nextDependents, list);

            nextDependents = new HashSet<string>[] { new HashSet<string>() {"c3"},
                new HashSet<string>() {"d4", "b2"}};

            list = s.SetCellContents("c3", new Formula("goober - c9"));
            CheckRecalculateList(nextDependents, list);

            nextDependents = new HashSet<string>[] { new HashSet<string>() {"a1"},
                new HashSet<string>() {"b2"},
                new HashSet<string>() {"d4"}};

            list = s.SetCellContents("a1", "this is the end of the test");
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
                s.SetCellContents("b1", new Formula("a1"));
            }
            catch(CircularException c)
            {
                noError = false;
                CheckRecalculateList("a1", s.SetCellContents("a1", new Formula("a2 + a9")));
                CheckRecalculateList(nextDependents, s.SetCellContents("b2", 3));

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

            CheckRecalculateList(nextDependents, s.SetCellContents("d4", 3));

            nextDependents = new HashSet<string>[]
            {
                new HashSet<string>() {"b2"},
                new HashSet<string>() {"a1"}
            };

            CheckRecalculateList(nextDependents, s.SetCellContents("b2", ""));
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().Contains("b2"));

            CheckRecalculateList("e5", s.SetCellContents("e5", ""));
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().Contains("e5"));
        }
        */
    }
}
