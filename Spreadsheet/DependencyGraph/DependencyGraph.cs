// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {

        private class Node
        {
            private HashSet<string> dependents;
            private HashSet<string> dependees;
            private string name;

            public Node (string n)
            {
                name = n;
                dependents = new HashSet<string>();
                dependees = new HashSet<string>();
            }

            public bool AddDependent (string dependent)
            {
                return dependents.Add(dependent);
            }

            public bool RemoveDependent (string dependent)
            {
                return dependents.Remove(dependent);
            }

            public bool AddDependee (string dependee)
            {
                return dependees.Add(dependee);
            }

            public bool RemoveDependee (string dependee)
            {
                return dependees.Remove(dependee);
            }

            public override string ToString()
            {
                return name;
            }

            public HashSet<string> GetDependents()
            {
                return dependents;
            }

            public HashSet<string> GetDependees()
            {
                return dependees;
            }
            
        }

        private Dictionary<string, Node> nodes;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            nodes = new Dictionary<string, Node>();
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return nodes.Count; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get { return nodes[s].GetDependees().Count; }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            return nodes[s].GetDependents().Count == 0;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            return nodes[s].GetDependees().Count == 0;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            return nodes[s].GetDependents();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            return nodes[s].GetDependees();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            Node nodeS = AddNode(s);
            Node nodeT = AddNode(t);

            nodeS.AddDependent(t);
            nodeT.AddDependee(s);
        }

        private Node AddNode(string name)
        {
            if (! nodes.ContainsKey(name))
            {
                Node node = new Node(name);
                nodes[name] = node;
                return node;
            }

            return nodes[name];
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            nodes[s].RemoveDependent(t);
            nodes[t].RemoveDependee(s);
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            Node nodeS = nodes[s];
            foreach (string oldDependent in nodeS.GetDependents())
            {
                nodes[oldDependent].RemoveDependee(s);
            }
            nodeS.GetDependents().Clear();

            foreach (string newDependent in newDependents)
            {
                nodeS.AddDependent(s);
                AddNode(newDependent).AddDependee(s);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            Node nodeS = nodes[s];
            foreach (string oldDependee in nodeS.GetDependees())
            {
                nodes[oldDependee].RemoveDependent(s);
            }
            nodeS.GetDependees().Clear();

            foreach (string newDependee in newDependees)
            {
                nodeS.AddDependee(s);
                AddNode(newDependee).AddDependent(s);
            }
        }

    }

}
