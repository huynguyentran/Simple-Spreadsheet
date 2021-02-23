using System;
using System.Diagnostics;
using System.Collections.Generic;
using FormulaEvaluator;

namespace FormulaEvaluatorTester
{
    /// <summary>
    /// A class that tests the FormulaEvaluator Class through the command prompt.
    /// </summary>
    static class Tester
    {
        private delegate void TestSet();

        private static bool rememberVars = true;

        private static Dictionary<string, int> vars = new Dictionary<string, int>();

        private readonly static TestSet[] tests = new TestSet[] { SyntaxErrorTests, BasicArithmetic, Parentheses, Variables};

        private readonly static Dictionary<string, int> NOVARS = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            string input = Console.In.ReadLine(); ;
            while (!input.Equals("quit") && !input.Equals("stop"))
            {
                vars.Clear();

                if (input.Length > 4 && input.Substring(0, 4).Equals("run "))
                {
                    string rest = input.Substring(4, input.Length - 4).Trim();
                    if (!rest.Equals("all"))
                    {
                        int index = int.Parse(rest);
                        if (index >= 0 && index < tests.Length)
                            tests[index]();
                    }
                    else
                    {
                        foreach (TestSet t in tests)
                            t();
                    }
                }
                else
                {
                    try
                    {
                        Console.WriteLine(Evaluator.Evaluate(input, AskVar));
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }
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
                Console.WriteLine("The Evaluator incorrectly solved the expression \"" + expression + "\" to be " + solution + " instead of " + answer + ".\n");
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
                solution = Evaluator.Evaluate(expression, HaveVar);
                Console.Out.WriteLine("The Evaluator unexpectantly finished with a solution of " + solution + " for expression \"" + expression + "\".\n");
            }
            catch(E specificError)
            {
                Console.Out.WriteLine("The Evaluator correctly encountered an error of type " + specificError.GetType() + " for expression \"" + expression + "\":\n");
                Console.Out.WriteLine(specificError.Message + "\n");
            }
            catch(Exception e)
            {
                Console.Out.WriteLine("The Evaluator unexpectantly encountered an error of type " + e.GetType() + " for expression \"" + expression + "\".\n");
                Console.Out.WriteLine(e.Message + "\n");
                Console.Out.WriteLine(e.StackTrace);
            }
        }

        private static void SyntaxErrorTests()
        {
            ErrorTest<ArgumentException>("1 7 +", NOVARS);
            ErrorTest<ArgumentException>("/ 3 6", NOVARS);
            ErrorTest<ArgumentException>("-3", NOVARS);
            ErrorTest<ArgumentException>("/ 32", NOVARS);
            ErrorTest<ArgumentException>("7 +", NOVARS);
        }

        private static void BasicArithmetic()
        {
            int i = 5;

            string[] expressions = new string[] { "1+2", "3 * 2", "7/ 4", "2-4", "3 + 7 + 9"};
            Dictionary<string, int>[] dictionaries = new Dictionary<string, int>[i];
            int[] answers = new int[] { 3, 6, 1, -2, 19 };

            for (int e = 0; e < i; e++)
                dictionaries[e] = NOVARS;

            CheckTests(expressions, dictionaries, answers);

            ErrorTest<ArgumentException>("3 / 0", NOVARS);
            ErrorTest<ArgumentException>("(8 + 5) / (9 *(8/2 - 4))", NOVARS);
        }

        private static void Parentheses()
        {
            ErrorTest<ArgumentException>("3 + ( 9 - 2", NOVARS);
            ErrorTest<ArgumentException>("3 + (9 - 2 ))", NOVARS);
            ErrorTest<ArgumentException>("3 + (9 - 2) * ((3 - 7) * (12)", NOVARS);

            CheckTest("3 * (4 + 2)", NOVARS, 18);
            CheckTest("3 + (7 - 9) * ((6 - 7) * (12))", NOVARS, 27);
        }

        private static void Variables()
        {
            ErrorTest<ArgumentException>("1 + A", NOVARS);
            ErrorTest<ArgumentException>("2 + AAB3A * 3", NOVARS);
            ErrorTest<ArgumentException>("GT", NOVARS);
            ErrorTest<ArgumentException>("2* (0 + 7A)", NOVARS);

            CheckTest("3 + A1", new Dictionary<string, int>() { { "A1", 2 } }, 5);
            CheckTest("3*(BBC456) - C66", new Dictionary<string, int>() { { "BBC456", 2 }, {"C66", 2} }, 4);
        }
    }
}
