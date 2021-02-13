using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EmptyConstructor()
        {
            Formula f = new Formula("");
        }


        private static string RemoveWhiteSpace(string s)
        {
            Regex r = new Regex(@"\s");
            return r.Replace(s, "");
        }

        private static void TestShareVariables(HashSet<string> expected, IEnumerable<string> received)
        {
            foreach (string variable in received)
            {
                if (! expected.Remove(variable))
                {
                    Assert.Fail("Unexpected variable " + variable + " received.");
                }
            }

            string left = "";

            foreach (string variable in expected)
                left += " " + variable;


            if (expected.Count > 0)
                Assert.Fail("Missing variables" + left + ".");
        }

        private static Formula SimpleTest(string tokens, Func<string, string> normalizer, Func<string, bool> validator, double evaluation, Func<string, double> lookup)
        {
            Formula f = new Formula(tokens, normalizer, validator);

            Regex r = new Regex(@"[a-zA-Z_]\w*");
            
            string[] nonVars = r.Split(tokens);
            IEnumerable<Match> matches = r.Matches(tokens);
            HashSet<string> expectedVariables = new HashSet<string>();
            tokens = "";
            int i = 0;

            foreach (Match m in matches)
            {
                string normalized = normalizer(m.Value);
                tokens += nonVars[i++] + normalized;
                expectedVariables.Add(normalized);
            }

            tokens += nonVars[i];

            Assert.AreEqual(evaluation, (double) f.Evaluate(lookup), 1e-9);

            Assert.AreEqual(RemoveWhiteSpace(tokens), f.ToString());

            TestShareVariables(expectedVariables, f.GetVariables());

            return f;
        }

        private static Formula SimpleTest(string tokens, double evaluation)
        {
            return SimpleTest(tokens, s => s, s => false, evaluation, s => 0);
        }

        private static void ExpectFormulaError(string tokens, Func<string, string> normalizer, Func<string, bool> validator, Func<string, double> lookup)
        {
            Formula f = new Formula(tokens, normalizer, validator);
            Assert.IsInstanceOfType(f.Evaluate(lookup), typeof(FormulaError));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void WhiteSpaceConstructor()
        {
            Formula f = new Formula("        ");
        }

        [TestMethod]
        public void SimpleAdditionConstructor()
        {
            SimpleTest("2+2", 4d);
            SimpleTest("2+ 7+3", 12d);
            SimpleTest("2 + 5 + 7 + 3", 17d);
            SimpleTest("1.66 + 7.9 + 2", 11.56);
        }

        [TestMethod]
        public void SimpleSubtractionConstructor()
        {
            SimpleTest("3-5", -2d);
            SimpleTest("5- 3-1", 1d);
            SimpleTest("5 - 3 - 2", 0d);
            SimpleTest("2.5 - 2 - 0.3", 0.2);
            SimpleTest("3.5 - 2 - 0.3 - 0.2", 1d);
        }

        [TestMethod]
        public void SimpleMultiplicationConstructor()
        {
            SimpleTest("2*3", 6d);
            SimpleTest("2* 3*9", 54d);
            SimpleTest("2 * 3 * 2", 12d);
            SimpleTest("2 * 3 * 0.5", 3d);
        }

        [TestMethod]
        public void SimpleDivisionConstructor()
        {
            SimpleTest("6/2", 3d);
            SimpleTest("6/4", 1.5d);
            SimpleTest("36 / 6 / 9", 0.666666666);
        }

        [TestMethod]
        public void DivisionByZeroConstructor()
        {
            Formula f = new Formula("6/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
            Assert.IsFalse(f.GetVariables().GetEnumerator().MoveNext());
            Assert.AreEqual("6/0", f.ToString());
        }

        [TestMethod]
        public void DivisionByZeroParenthesisConstructor()
        {
            Formula f = new Formula(" 1 / (0)");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
            Assert.IsFalse(f.GetVariables().GetEnumerator().MoveNext());
            Assert.AreEqual("1/(0)", f.ToString());
        }

        [TestMethod]
        public void SimpleParenthesesConstructor()
        {
            SimpleTest("(1+7)/2", 4d);
            SimpleTest("((1+7) + 2) - (8 * (0.5)) * 2", 2d);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadLeftParenthesesConstructor()
        {
            Formula f = new Formula("(9) + 3 * ((4.785498)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadRightParenthesesConstructor()
        {
            Formula f = new Formula("(7 - (83 - 9)) * 3) - 1");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadEmptyParenthesesConstructor()
        {
            Formula f = new Formula("1 + 7 + (3 - 2 ()) / 7");
        }

        [TestMethod]
        public void SimpleVariablesEvaluation()
        {
            /*
             * Variables are capital letters followed by digits. Letters are automatically capitalized.
             * The value of the variable corresponds to the number made by its digits.
             * e.g. ACDR64 has the value of 64.
             */
            Func<string, double> lookup = s => Double.Parse(new Regex(@"\d+$").Match(s).Value);
            Func<string, bool> validator = s => new Regex(@"^[A-Z]+\d+$").IsMatch(s);
            Func<string, string> normalizer = s => s.ToUpper();

            SimpleTest("1 + a1", normalizer, validator, 2, lookup);

            SimpleTest("1 + (a5 - 2) * sfdSgklj2", normalizer, validator, 7, lookup);
            
            SimpleTest("1 / arg2 * (bfs3 * (TURBO2 / three3))", normalizer, validator, 1, lookup);
        }

        [TestMethod]
        public void VariableDivisionByZero()
        {
            Func<string, double> lookup = s => 0;
            Func<string, bool> validator = s => new Regex(@"^[A-Z]+\d+$").IsMatch(s);
            Func<string, string> normalizer = s => s.ToUpper();

            ExpectFormulaError("1 / a1", normalizer, validator, lookup);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IncompatibleVariableName()
        {
            Func<string, bool> validator = s => new Regex(@"^[A-Z]+\d+$").IsMatch(s);
            Func<string, string> normalizer = s => s;

            new Formula("2 - (AASJFDbertAKSLF114 + 7) * 9", normalizer, validator);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IncompleteVariableName()
        {
            Func<string, bool> validator = s => new Regex(@"^[A-Z]+\d+$").IsMatch(s);
            Func<string, string> normalizer = s => s;

            new Formula("2 - (AASJFDAKSLF + 7) * 9", normalizer, validator);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void UnrecognizedToken()
        {
            new Formula("(3 - 7)*12 - (4.2 + ;ibberish)", s => s, s => true);
        }

        [TestMethod]
        public void SimpleEquals()
        {
            Func<string, double> lookup = s => Double.Parse(new Regex(@"\d+$").Match(s).Value);
            Func<string, bool> validator = s => new Regex(@"^[A-Z]+\d+$").IsMatch(s);
            Func<string, string> normalizer = s => s.ToUpper();

            Formula f1 = SimpleTest("1 + 2 / (1 * (guac4 - 6))", normalizer, validator, 0 ,lookup);
            Formula f2 = SimpleTest("1 + 4 / (1 * (guac4 - 6))", normalizer, validator, -1, lookup);
            Formula f3 = SimpleTest("1 + 2 / (1 * (duck4 - 6))", normalizer, validator, 0, lookup);
            Formula f4 = SimpleTest("1 + 2 / ((guac4 - 6) * 1)", normalizer, validator, 0, lookup);
            Formula f5 = SimpleTest("  1+ 2 /   (1*(guac4-6 )   )", normalizer, validator, 0, lookup);

            Assert.AreNotEqual(f1, f2);
            Assert.AreNotEqual(f1.GetHashCode(), f2.GetHashCode());
            Assert.AreNotEqual(f1, f3);
            Assert.AreNotEqual(f1.GetHashCode(), f3.GetHashCode());
            Assert.AreNotEqual(f1, f4);
            Assert.AreNotEqual(f1.GetHashCode(), f4.GetHashCode());
            Assert.AreEqual(f1, f5);
            Assert.AreEqual(f1.GetHashCode(), f5.GetHashCode());
            Assert.AreEqual(f1, f1);
            Assert.AreEqual(f1.GetHashCode(), f1.GetHashCode());

            Assert.IsFalse(f1 == f2);
            Assert.IsFalse(f1 == f3);
            Assert.IsFalse(f1 == f4);
            Assert.IsTrue(f1 == f5);
            Assert.IsTrue(f1 == f1);

            Assert.IsTrue(f1 != f2);
            Assert.IsTrue(f1 != f3);
            Assert.IsTrue(f1 != f4);
            Assert.IsFalse(f1 != f5);
            Assert.IsFalse(f1 != f1);
        }

        [TestMethod]
        public void NullEquals()
        {
            Func<string, double> lookup = s => Double.Parse(new Regex(@"\d+$").Match(s).Value);
            Func<string, bool> validator = s => new Regex(@"^[A-Z]+\d+$").IsMatch(s);
            Func<string, string> normalizer = s => s.ToUpper();

            Formula f1 = SimpleTest("0 * ((93 - holdit147) * 89 - question2 / 80)", normalizer, validator, 0, lookup);
            Formula f2 = null;
            Formula f3 = null;

            Assert.IsFalse(f1.Equals(f2));
            Assert.IsFalse(f1 == f2);
            Assert.IsFalse(f2 == f1);
            Assert.IsTrue(f1 != f2);
            Assert.IsTrue(f2 != f1);

            Assert.IsTrue(f2 == f2);
            Assert.IsTrue(f2 == f3);
            Assert.IsFalse(f2 != f2);
            Assert.IsFalse(f2 != f3);

        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadStart()
        {
            new Formula(" / (1 - 3 + 1)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MissingBackOperand()
        {
            new Formula(" 1 + (2 -) * 3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MissingFrontOperand()
        {
            new Formula("42 / 7 + - 9");
        }

        [TestMethod]
        public void MissingVariable()
        {
            Func<string, double> lookup = s => throw new ArgumentException("I don't know any variables :(");
            Func<string, bool> validator = s => new Regex(@"^[A-Z]+\d+$").IsMatch(s);
            Func<string, string> normalizer = s => s.ToUpper();

            ExpectFormulaError("0 * ((93 - holdit147) * 89 - question2 / 80)", normalizer, validator, lookup);
        }
    }
}
