using System;
using System.Diagnostics;
using System.Collections.Generic;
using FormulaEvaluator;

namespace FormulaEvaluatorTester
{
    class Tester
    {
        private delegate void TestSet();

        private static bool rememberVars = true;

        private static Dictionary<string, int> vars = new Dictionary<string, int>();

        private readonly static TestSet[] tests;

        static void Main(string[] args)
        {
            string input = "";
            while (!input.Equals("quit"))
            {
                input = Console.In.ReadLine();

                vars.Clear();

                if (input.Length > 4 && input.Substring(0, 4).Equals("run "))
                {
                    tests[int.Parse(input.Substring(4, input.Length))]();
                }
                else
                {
                    Console.WriteLine(Evaluator.Evaluate(input, AskVar));
                }
            }
        }

        static int AskVar(string var)
        {
            if (rememberVars)
            {
                if (!vars.ContainsKey(var))
                {
                    Console.Out.WriteLine("What is the value of " + var + ".");
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
                Console.WriteLine("The Evaluator correctly solved the expression \"" + expression + "\" to be " + answer);
            }
            else
            {
                Console.WriteLine("The Evaluator inccorrectly solved the expression \"" + expression + "\" to be " + solution + " instead of " + answer);
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
                Console.Out.WriteLine("The Evaluator correctly encountered an error of type " + specificError.GetType() + " for expression \"" + expression + "\"");
            }
            catch(Exception e)
            {
                Console.Out.WriteLine("The Evaluator unexpectantly encountered an error of type " + e.GetType() + " for expression \"" + expression + "\"");
                Console.Out.WriteLine(e.StackTrace);
            }

            Console.Out.WriteLine("The Evaluator unexpectantly finished with a solution of " + solution + " for expression \"" + expression + "\"");
        }
    }
}
