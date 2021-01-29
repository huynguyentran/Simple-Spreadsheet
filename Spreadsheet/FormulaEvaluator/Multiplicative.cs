using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaEvaluator
{
    abstract class Multiplicative : Operator
    {
        public override int GetOperandCount()
        {
            return 2;
        }
    }

    class Times : Multiplicative
    {
        public override int DoOperation(int[] operands)
        {
            base.DoOperation(operands);
            return operands[0] * operands[1];
        }

        public override string ToString()
        {
            return "*";
        }
    }

    class Divide : Multiplicative
    {
        public override int DoOperation(int[] operands)
        {
            base.DoOperation(operands);

            if (operands[1] == 0)
                throw new ArgumentException("Recieved the arguments " + operands[0] + " / " + operands[1] + " for division; cannot divide by zero.");
            
            return operands[0] / operands[1];
        }

        public override string ToString()
        {
            return "/";
        }
    }
}
