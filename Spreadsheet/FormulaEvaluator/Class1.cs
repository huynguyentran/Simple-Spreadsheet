using System;

namespace FormulaEvaluator
{
    public static class Evaluator
    {
        public delegate int Lookup(String v);

        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            return 0;
        }


        private static bool IsNumber(String sequence, int startIndex)
        {
            if (startIndex > sequence.Length || startIndex < 0)
                throw new IndexOutOfRangeException("A number was asked to be identified from an index that did not exist within the length of the sequence to check.");
            else if (startIndex == sequence.Length)
                return false;

            while (startIndex < sequence.Length)
                if (!Char.IsNumber(sequence[startIndex++]))
                    return false;

            return true;
        }

        private static bool IsNumber(String sequence)
        {
            return IsNumber(sequence, 0);
        }

        private static bool IsVariable (String sequence)
        {
            if (sequence.Length < 2)
                return false;

            int i = 0;
            while (i < sequence.Length && Char.IsLetter(sequence[i++])) {}

            return i > 0 && IsNumber(sequence, i);
        }

        private static bool IsValue(string token, Lookup variableEvaluator, out int value)
        {

            value = -1;

            if (IsNumber(token))
            {
                value = int.Parse(token);
                return true;
            }
            else if (IsVariable(token))
            {
                value = variableEvaluator(token);
                return true;
            }

            return false;
        }
    }
}
