using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


namespace DevelopmentTests
{
    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }



        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }



        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

        //William's Tests:
        [TestMethod]
        public void EmptyDependentInfo()
        {
            DependencyGraph dg = new DependencyGraph();
            Assert.AreEqual(0, dg.Size);
            Assert.AreEqual(0, dg["c"]);
            Assert.IsFalse(dg.HasDependents("d"));
            IEnumerator<string> dependents = dg.GetDependents("e").GetEnumerator();
            Assert.IsFalse(dependents.MoveNext());
        }

        [TestMethod]
        public void EmptyDependeeInfo()
        {
            DependencyGraph dg = new DependencyGraph();
            Assert.AreEqual(0, dg.Size);
            Assert.IsFalse(dg.HasDependees("d"));
            IEnumerator<string> dependees = dg.GetDependees("e").GetEnumerator();
            Assert.IsFalse(dependees.MoveNext());
        }

        private void TestEnumerator<T> (IEnumerator<T> enumerator, List<T> expected)
        {
            while (enumerator.MoveNext())
            {
                bool foundExpected = false;
                for(int i = 0; i < expected.Count; i++)
                {
                    if(enumerator.Current.Equals(expected[i]))
                    {
                        expected.RemoveAt(i);
                        foundExpected = true;
                        break;
                    }
                }
                if (!foundExpected)
                    Assert.Fail("Expected an element of " + expected + " but got " + enumerator.Current);
            }
        }

        [TestMethod]
        public void SmallDependencyGraphDependentInfo()
        {
            DependencyGraph dg = new DependencyGraph();

            dg.AddDependency("Taco Del Mar", "me");
            dg.AddDependency("Blender", "me");
            dg.AddDependency("Unity", "me");
            dg.AddDependency("Blender", "Unity");

            Assert.AreEqual(4, dg.Size);
            Assert.AreEqual(0, dg["me"]);
            Assert.AreEqual(1, dg["Unity"]);
            Assert.IsFalse(dg.HasDependents("me"));
            Assert.IsTrue(dg.HasDependents("Taco Del Mar"));

            TestEnumerator(dg.GetDependents("Blender").GetEnumerator(), new List<string>(new string[] { "me", "Unity" }));
        }

        [TestMethod]
        public void SmallDependencyGraphDependeeInfo()
        {
            DependencyGraph dg = new DependencyGraph();

            dg.AddDependency("Taco Del Mar", "me");
            dg.AddDependency("Blender", "me");
            dg.AddDependency("Unity", "me");
            dg.AddDependency("Blender", "Unity");

            Assert.AreEqual(4, dg.Size);
            Assert.IsTrue(dg.HasDependees("me"));
            Assert.IsFalse(dg.HasDependees("Taco Del Mar"));

            TestEnumerator(dg.GetDependees("me").GetEnumerator(), new List<string>(new string[] { "Taco Del Mar", "Blender", "Unity" }));
        }

        private void TestNode(DependencyGraph dg, string node, string[] dependents, string[] dependees)
        {
            Assert.AreEqual(dependents.Length, dg[node]);

            Assert.AreEqual(dependents.Length > 0, dg.HasDependents(node));
            Assert.AreEqual(dependees.Length > 0, dg.HasDependees(node));

            TestEnumerator(dg.GetDependents(node).GetEnumerator(), new List<string>(dependents));
            TestEnumerator(dg.GetDependees(node).GetEnumerator(), new List<string>(dependees));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DependencyNullNodeWorks()
        {
            DependencyGraph dg = new DependencyGraph();
            //Dictionaries can't take null as a key, so an error will be thrown.
            dg.AddDependency(null, "what");
        }

        [TestMethod]
        public void SimpleRemoveTest()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("b", "c");
            dg.AddDependency("c", "a");

            dg.RemoveDependency("a", "b");

            TestNode(dg, "a", new string[] { }, new string[] {"c"});
            TestNode(dg, "b", new string[] {"c"}, new string[] { });
            TestNode(dg, "c", new string[] {"a"}, new string[] {"b"});
        }

        [TestMethod]
        public void RemoveNonexistentPairsTest()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.RemoveDependency("a", "b");
            Assert.AreEqual(0, dg.Size);

            dg.AddDependency("a", "c");
            dg.RemoveDependency("a", "b");
            Assert.AreEqual(1, dg.Size);

            dg.RemoveDependency("d", "c");
            Assert.AreEqual(1, dg.Size);
        }

        [TestMethod]
        public void EmptyReplace()
        {
            DependencyGraph dg = new DependencyGraph();
            
            dg.ReplaceDependents("a", new string[] { "t", "r", "f", "q" });
            dg.ReplaceDependees("c", new string[] { "9", "y", "a" });

            TestNode(dg, "a", new string[] { "t", "r", "f", "q", "c" }, new string[0]);
            TestNode(dg, "c", new string[0], new string[] { "9", "y", "a" });
        }

        /// <summary>
        /// Replacing a relationship that already exists.
        /// </summary>
        [TestMethod]
        public void ReReplace()
        {
            DependencyGraph dg = new DependencyGraph();

            dg.AddDependency("a", "b");
            dg.AddDependency("c", "b");
            dg.AddDependency("d", "b");

            dg.ReplaceDependees("b", new string[] { "d" });

            TestNode(dg, "b", new string[] { }, new string[] {"d"});

            dg.ReplaceDependents("d", new string[] { "b" });

            TestNode(dg, "d", new string[] { "b" }, new string[] { });

            dg.RemoveDependency("d", "b");

            TestNode(dg, "b", new string[] { }, new string[] { });
            TestNode(dg, "d", new string[] { }, new string[] { });

        }

        [TestMethod]
        public void SelfDependent()
        {
            for (int i = 0; i < 3; i++)
            {
                DependencyGraph dg = new DependencyGraph();

                dg.AddDependency("1", "1");

                TestNode(dg, "1", new string[] { "1" }, new string[] { "1" });
                
                switch (i)
                {
                    case 0:
                        dg.ReplaceDependents("1", new string[] { });
                        break;
                    case 1:
                        dg.ReplaceDependees("1", new string[] { });
                        break;
                    case 2:
                        dg.RemoveDependency("1", "1");
                        break;
                }

                TestNode(dg, "1", new string[0], new string[0]);
            }
        }
    }
}