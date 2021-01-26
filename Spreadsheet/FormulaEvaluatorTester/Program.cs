﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using FormulaEvaluator;

namespace FormulaEvaluatorTester
{
    static class Tester
    {
        private delegate void TestSet();

        private static bool rememberVars = true;

        private static Dictionary<string, int> vars = new Dictionary<string, int>();

        private readonly static TestSet[] tests = new TestSet[] {BasicErrorTests};

        private readonly static Dictionary<string, int> NOVARS;

        static void Main(string[] args)
        {
            string input = Console.In.ReadLine(); ;
            while (!input.Equals("quit") && !input.Equals("stop"))
            {
                vars.Clear();

                if (input.Length > 4 && input.Substring(0, 4).Equals("run "))
                {
                    int index = int.Parse(input.Substring(4, input.Length - 4));
                    if (index >= 0 && index < tests.Length)
                        tests[index]();
                }
                else
                {
                    Console.WriteLine(Evaluator.Evaluate(input, AskVar));
                }

                input = Console.In.ReadLine();
            }
        }

        static int AskVar(string var)
        {
            if (rememberVars)
            {
                if (!vars.ContainsKey(var))
                {
                    Console.Out.WriteLine("What is the value of " + var + "?\n");
                    vars.Add(var, int.Parse(Console.In.ReadLine()));
                }

                return vars[var];
            }
            else
            {
                return int.Parse(Console.In.ReadLine());
            }
        }

        static int HaveVar(string var)
        {
            return vars[var];
        }

        private static void CheckTest(string expression, Dictionary<string, int> _vars, int answer)
        {
            vars = _vars;
            int solution = Evaluator.Evaluate(expression, HaveVar);
            if (solution == answer)
            {
                Console.WriteLine("The Evaluator correctly solved the expression \"" + expression + "\" to be " + answer + ".\n");
            }
            else
            {
                Console.WriteLine("The Evaluator inccorrectly solved the expression \"" + expression + "\" to be " + solution + " instead of " + answer + ".\n");
            }
        }

        private static void CheckTests(string[] expressions, Dictionary<string, int>[] _vars, int[] answers)
        {
            Debug.Assert(expressions.Length == _vars.Length);
            Debug.Assert(_vars.Length == answers.Length);

            for (int i = 0; i < expressions.Length; i++)
            {
                CheckTest(expressions[i], _vars[i], answers[i]);
            }
        }

        private static void ErrorTest<E>(string expression, Dictionary<string, int> _vars) where E : Exception
        {
            int solution = -1;
            try
            {
                solution = Evaluator.Evaluate(expression, HaveVar); ;
            }
            catch(E specificError)
            {
                Console.Out.WriteLine("The Evaluator correctly encountered an error of type " + specificError.GetType() + " for expression \"" + expression + "\".\n");
            }
            catch(Exception e)
            {
                Console.Out.WriteLine("The Evaluator unexpectantly encountered an error of type " + e.GetType() + " for expression \"" + expression + "\".\n");
                Console.Out.WriteLine(e.StackTrace);
            }

            Console.Out.WriteLine("The Evaluator unexpectantly finished with a solution of " + solution + " for expression \"" + expression + "\".\n");
        }

        private static void BasicErrorTests()
        {
            ErrorTest<ArgumentException>("1 7 +", NOVARS);
            ErrorTest<ArgumentException>("/ 3 6", NOVARS);
        }
    }
}
