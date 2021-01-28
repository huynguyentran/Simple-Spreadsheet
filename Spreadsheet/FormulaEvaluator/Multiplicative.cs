using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaEvaluator
{
    abstract class Multiplicative : Operator
    {
    }

    class Times : Multiplicative
    {
        public override int DoOperation(int v1, int v2)
        {
            return v1 * v2;
        }

        public override bool IsOperator(string token)
        {
            return "*".Equals(token);
        }
    }

    class Divide : Multiplicative
    {
        public override int DoOperation(int v1, int v2)
        {
            return v1 / v2;
        }

        public override bool IsOperator(string token)
        {
            return "/".Equals(token);
        }
    }
}
