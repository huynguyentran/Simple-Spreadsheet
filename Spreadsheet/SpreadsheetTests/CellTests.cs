using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System;

namespace SpreadsheetTests
{
    [TestClass]
    public class CellTests
    {
        [TestMethod]
        public void CorrectCasting()
        {
            Cell str = new Cell("string");
            Cell doub = new Cell(400413d);

            Assert.AreEqual("string", str.Contents);
            Assert.IsTrue(str.CanCast(out string strCont));
            Assert.AreEqual("str", strCont.Substring(0, 3));

            Assert.AreEqual(400413d, doub.Contents);
            Assert.IsTrue(doub.CanCast(out double doubCont));
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
            Assert.IsFalse(str.CanCast(out double doubCont));

            Assert.AreEqual(400413d, doub.Contents);
            Assert.IsFalse(doub.CanCast(out string strCont));
        }
    }
}
