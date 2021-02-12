// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>

    public delegate bool ValidateAndNormalize(ref string token);

    public class Formula
    {
        private string stringRep;

        private Func<string, string> normalizer;
        private Func<string, bool> validator;

        private static readonly Regex variableRegex = new Regex(@"^[a-zA-Z_][a-zA-Z_0-9]*$");
        private static readonly FormulaOperator[] operators = new FormulaOperator[] {new Plus(), new Minus(), new Times(), new Divide(), new LeftParenthesis(), new RightParenthesis()};

        private readonly HashSet<String> variables = new HashSet<string>();

        private readonly LinkedList<FormulaElement> formulaElements = new LinkedList<FormulaElement>();

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            stringRep = "";
            normalizer = normalize;
            validator = isValid;

            ValidateAndNormalize[] valueConverters = new ValidateAndNormalize[] { IsVariable, IsDecimal };

            Stack<FormulaOperator> parentheses = new Stack<FormulaOperator>();
            
            foreach(string token in GetTokens(formula))
            {
                string tempToken = token;
                FormulaElement current = null;

                if (IsValue(ref tempToken, valueConverters))
                {
                    stringRep += tempToken;
                    current = new Value(tempToken);
                }
                else if(IsOperator(tempToken, out FormulaOperator op))
                {
                    stringRep += op.ToString();
                    current = op;
                    if (op is Parenthetical p)
                        p.HandleStacks(parentheses);
                }

                bool isValidFollow = false;
                string errorMessage;

                if (formulaElements.Count == 0)
                {
                    isValidFollow = (current is Value) || (current is LeftParenthesis);
                    errorMessage = "A " + current + " of type " + current.GetType() + 
                        " cannot start a formula. Is a value missing at the beginning?";
                }
                else
                {
                    FormulaElement last = formulaElements.Last.Value;

                    isValidFollow = last.CanFollow(current);
                    errorMessage = "A " + current + " of type " + current.GetType() +
                        " cannot follow a " + last + " of type " + last.GetType() +". Is an operator missing an operand, or is there a value without an operator?";
                }

                if (isValidFollow)
                {
                    formulaElements.AddLast(current);
                }
                else
                    throw new FormulaFormatException(errorMessage);
            }

            if (parentheses.Count > 0)
                throw new FormulaFormatException(parentheses.Count + " left parentheses are missing right pairs. Do all parentheses have their pairs?");

            if (formulaElements.Count < 0 || stringRep == "")
                throw new FormulaFormatException("No tokens were detected. Are you sure anything is written?");
        }

        private bool IsVariable(ref string token)
        {   
            if (variableRegex.IsMatch(token))
            {
                string tempToken = normalizer(token);
                if (validator(tempToken))
                {
                    token = tempToken;
                    variables.Add(token);
                    return true;
                }
            }

            return false;
        }

        private static bool IsDecimal(ref string token)
        {
            if (Double.TryParse(token, out double doubleRep))
            {
                token = doubleRep.ToString();
                return true;
            }
            return false;
        }

        private static bool IsValue(ref string token, ValidateAndNormalize[] valueConverters)
        {
            foreach(ValidateAndNormalize vAndN in valueConverters)
            {
                if (vAndN(ref token))
                    return true;
            }

            return false;
        }

        private bool IsOperator(string token, out FormulaOperator op)
        {
            op = null;
            foreach (FormulaOperator possibleOp in operators)
            {
                if (possibleOp.IsOperator(token))
                {
                    op = possibleOp;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<double> values = new Stack<double>();
            Stack<FormulaOperator> operators = new Stack<FormulaOperator>();

            foreach(FormulaElement element in formulaElements)
            {
                if (element is Value v)
                {
                    double doubleElement;
                    string strElement = element.ToString();
                    if (! Double.TryParse(strElement, out doubleElement))
                    {
                        try
                        {
                            doubleElement = lookup(strElement);
                        }
                        catch(ArgumentException e)
                        {
                            return new FormulaError("Couldn't find the value of variable " + strElement + ":\n" +e.Message);
                        }
                    }

                    if (operators.IsOnTop<FormulaOperator, Multiplicative>())
                    {
                        object multiplicationResult;
                        if (values.TryPop(out double valueLeft))
                            multiplicationResult = operators.Pop().DoOperation(new double[] { valueLeft, doubleElement });
                        else
                            throw new ArgumentException("The formula" + stringRep + "found a missing operand for addition after declaring itself valid.");

                        if (multiplicationResult is double d)
                            doubleElement = d;
                        else
                            return multiplicationResult;
                    }

                    values.Push(doubleElement);
                }
                else if (element is FormulaOperator op)
                {
                    if (!op.HandleStacks(values, operators, out OperatorError opError))
                    {
                        if (ReferenceEquals(opError, null))
                            throw new ArgumentException("An operator failed to handle the stacks without giving a reason.");

                        return opError.error;
                    }
                }
                else
                {
                    throw new ArgumentException("The formula" + stringRep +  "found an unrecognized FormulaOperator " + element + " of type " + element.GetType() + ".");
                }
            }

            if (operators.Count == 1)
                if (!FormulaOperator.DoOperationIf<Additive>(values, operators, out object error))
                    throw new ArgumentException("The formula" + stringRep + " finished with an operator other than + or - after being validated");

            if (values.Count == 1 && operators.Count == 0)
                return values.Pop();
            else
                throw new ArgumentException("The formula" + stringRep + " finished with excess operators or values. Is a left parenthesis missing a right pair?");
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return stringRep;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Formula))
                return false;

            string other = (obj as Formula).stringRep;
            if (ReferenceEquals(stringRep, null))
                return ReferenceEquals(other, null);

            return stringRep.Equals(other);
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (ReferenceEquals(f1, null))
                return ReferenceEquals(f2, null);
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return stringRep.GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}

