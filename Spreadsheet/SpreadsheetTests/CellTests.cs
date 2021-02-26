using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System;

namespace SpreadsheetTests
{
    /// <author>William Erignac</author>
    /// <version>2/19/2021</version>
    [TestClass]
    public class CellTests
    {
        [TestMethod]
        public void CorrectCasting()
        {
            Cell str = new Cell("string");
            Cell doub = new Cell(400413d);

            Assert.AreEqual("string", str.Contents);
            Assert.IsTrue(str.TryCast(out string strCont));
            Assert.AreEqual("str", strCont.Substring(0, 3));

            Assert.AreEqual(400413d, doub.Contents);
            Assert.IsTrue(doub.TryCast(out double doubCont));
            Assert.AreEqual(400413d / 2, doubCont / 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullCell()
        {
            Cell c = new Cell(null);
        }

        [TestMethod]
        public void BadCanCast()
        {
            Cell str = new Cell("string");
            Cell doub = new Cell(400413d);

            Assert.AreEqual("string", str.Contents);
            Assert.IsFalse(str.TryCast(out double doubCont));

            Assert.AreEqual(400413d, doub.Contents);
            Assert.IsFalse(doub.TryCast(out string strCont));
        }

        [TestMethod]
        public void FormulaConstructor()
        {
            //Simple Formula.
            Formula formula = new Formula("(1+7)/4 - 8");
            Cell cell = new Cell(formula, s => 0);

            Assert.AreEqual(formula, cell.Contents);
            Assert.AreEqual((double) formula.Evaluate(s => 0), (double) cell.Value, 1e-9);

            //Formula with variable.
            formula = new Formula("a1 + 3");
            cell = new Cell(formula, s => 3);

            Assert.AreEqual(formula, cell.Contents);
            Assert.AreEqual((double)formula.Evaluate(s => 3), (double)cell.Value, 1e-9);

            //Formula Error from Division by 0.
            formula = new Formula("1 / (1 - 1)");
            cell = new Cell(formula, s => 0);

            Assert.AreEqual(formula, cell.Contents);
            Assert.AreEqual(formula.Evaluate(s => 0), cell.Value);

            //Formula Error from missing variable value.
            formula = new Formula("a1 + 3");
            cell = new Cell(formula, s => throw new ArgumentException());

            Assert.AreEqual(formula, cell.Contents);
            Assert.AreEqual(formula.Evaluate(s => throw new ArgumentException()), cell.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormulaConstructorNullFormula()
        {
            new Cell(null, s => 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormulaConstructorNullLookup()
        {
            new Cell(new Formula("a1 - 2 * 9 / b2"), null);
        }

        [TestMethod]
        public void UpdateValue()
        {
            double x = 2;
            Func<string, double> lookup = s => x;

            Cell cell = new Cell(new Formula("3*a1"), lookup);
            Assert.AreEqual(6d, (double)cell.Value, 1e-9);

            x = 3;
            Assert.IsTrue(cell.UpdateValue());
            Assert.AreEqual(9d, (double)cell.Value, 1e-9);
        }

        [TestMethod]
        public void UpdateValueFalse()
        {
            Cell cell = new Cell("This can't update, it's a string");
            Assert.IsFalse(cell.UpdateValue());
        }
    }
}
