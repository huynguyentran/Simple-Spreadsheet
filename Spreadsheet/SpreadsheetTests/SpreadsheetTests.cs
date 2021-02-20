using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        public void Empty()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("arbitrary"));
            IEnumerator<string> e = s.GetNamesOfAllNonemptyCells().GetEnumerator();
            Assert.IsFalse(e.MoveNext());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullNameDouble()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents(null, 15.32);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullNameString()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents(null, "Guac");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullNameFormula()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents(null, new Formula("3 - 2 + (9 * 8)"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void BadNameDouble()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("", 15.32);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void BadNameString()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("2x", "Guac");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void BadNameFormula()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("_&", new Formula("3 - 2 + (9 * 8)"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullString()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            string value = null;
            s.SetCellContents("Oh_this_is_bad", value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullFormula()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Formula value = null;
            s.SetCellContents("Zut_alors", value);
        }

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
    }
}
