using System;
using System.Globalization;
using System.Text;

namespace DifferentialEquationGrapherPrototype;

public class ExpressionParser
{
    private string input;
    private int position;
    private double xValue;
    private bool useTaylor;
    private int terms;
    private MathFunctions mathFunctions;

    public ExpressionParser(string expression, bool useTaylor, int terms)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new ArgumentException("Enter a function.");
        }

        if (terms < 1 || terms > 30)
        {
            throw new ArgumentException("The number of Taylor terms must be between 1 and 30.");
        }

        expression = expression.Replace('−', '-');
        expression = expression.Replace('×', '*');
        expression = expression.Replace('÷', '/');

        int equalsPosition = expression.IndexOf('=');

        if (equalsPosition >= 0)
        {
            if (expression.IndexOf('=', equalsPosition + 1) >= 0)
            {
                throw new ArgumentException("Only one equals sign is allowed.");
            }

            string leftSide = expression.Substring(0, equalsPosition).Trim();
            string rightSide = expression.Substring(equalsPosition + 1).Trim();

            if (leftSide == "y")
            {
                expression = rightSide;
            }
            else if (rightSide == "y")
            {
                expression = leftSide;
            }
            else
            {
                throw new ArgumentException("Use an equation in the form y = expression.");
            }
        }

        StringBuilder cleanedInput = new StringBuilder();

        foreach (char character in expression)
        {
            if (!char.IsWhiteSpace(character))
            {
                cleanedInput.Append(character);
            }
        }

        input = cleanedInput.ToString();
        this.useTaylor = useTaylor;
        this.terms = terms;
        mathFunctions = new MathFunctions();
    }

    public double Evaluate(double x)
    {
        position = 0;
        xValue = x;

        double answer = ParseExpression();

        if (position != input.Length)
        {
            throw new ArgumentException("There is an error in the expression.");
        }

        return answer;
    }

    private double ParseExpression()
    {
        double answer = ParseTerm();

        while (position < input.Length)
        {
            if (Match('+'))
            {
                answer = answer + ParseTerm();
            }
            else if (Match('-'))
            {
                answer = answer - ParseTerm();
            }
            else
            {
                break;
            }
        }

        return answer;
    }

    private double ParseTerm()
    {
        double answer = ParseUnary();

        while (position < input.Length)
        {
            if (Match('*'))
            {
                answer = answer * ParseUnary();
            }
            else if (Match('/'))
            {
                answer = answer / ParseUnary();
            }
            else if (CanStartImplicitMultiplication())
            {
                answer = answer * ParseUnary();
            }
            else
            {
                break;
            }
        }

        return answer;
    }

    private double ParseUnary()
    {
        if (Match('+'))
        {
            return ParseUnary();
        }

        if (Match('-'))
        {
            return -ParseUnary();
        }

        return ParsePower();
    }

    private double ParsePower()
    {
        double answer = ParsePrimary();

        if (Match('^'))
        {
            double exponent = ParseUnary();
            answer = mathFunctions.Power(answer, exponent);
        }

        return answer;
    }

    private double ParsePrimary()
    {
        if (position >= input.Length)
        {
            throw new ArgumentException("A value was expected.");
        }

        char current = input[position];

        if (current == '(')
        {
            position++;
            double answer = ParseExpression();
            Require(')');
            return answer;
        }

        if (char.IsDigit(current) || current == '.')
        {
            return ParseNumber();
        }

        if (current == 'x')
        {
            position++;
            return xValue;
        }

        if (char.IsLetter(current))
        {
            return ParseName();
        }

        throw new ArgumentException("An invalid character was found: " + current);
    }

    private double ParseNumber()
    {
        int start = position;
        bool hasDigit = false;

        while (position < input.Length && char.IsDigit(input[position]))
        {
            hasDigit = true;
            position++;
        }

        if (position < input.Length && input[position] == '.')
        {
            position++;

            while (position < input.Length && char.IsDigit(input[position]))
            {
                hasDigit = true;
                position++;
            }
        }

        if (!hasDigit)
        {
            throw new ArgumentException("Invalid number.");
        }

        if (position < input.Length && (input[position] == 'e' || input[position] == 'E'))
        {
            int exponentPosition = position + 1;

            if (exponentPosition < input.Length &&
                (input[exponentPosition] == '+' || input[exponentPosition] == '-'))
            {
                exponentPosition++;
            }

            if (exponentPosition < input.Length && char.IsDigit(input[exponentPosition]))
            {
                position = exponentPosition + 1;

                while (position < input.Length && char.IsDigit(input[position]))
                {
                    position++;
                }
            }
        }

        string numberText = input.Substring(start, position - start);
        double number;

        if (!double.TryParse(numberText, NumberStyles.Float, CultureInfo.InvariantCulture, out number))
        {
            throw new ArgumentException("Invalid number.");
        }

        return number;
    }

    private double ParseName()
    {
        int start = position;

        while (position < input.Length && char.IsLetter(input[position]))
        {
            position++;
        }

        string name = input.Substring(start, position - start);

        if (name == "e")
        {
            return Math.E;
        }

        if (name == "pi")
        {
            return Math.PI;
        }

        if (!IsFunction(name))
        {
            throw new ArgumentException("Unknown function: " + name);
        }

        Require('(');
        double value = ParseExpression();
        Require(')');

        return mathFunctions.Apply(name, value, useTaylor, terms);
    }

    private bool IsFunction(string name)
    {
        if (name == "sin") return true;
        if (name == "cos") return true;
        if (name == "tan") return true;
        if (name == "asin") return true;
        if (name == "acos") return true;
        if (name == "atan") return true;
        if (name == "sqrt") return true;
        if (name == "abs") return true;
        if (name == "ln") return true;
        if (name == "log") return true;
        if (name == "exp") return true;

        return false;
    }

    private bool CanStartImplicitMultiplication()
    {
        if (position >= input.Length)
        {
            return false;
        }

        char current = input[position];

        if (current == '(' || current == 'x' || char.IsLetter(current))
        {
            return true;
        }

        return false;
    }

    private bool Match(char wanted)
    {
        if (position < input.Length && input[position] == wanted)
        {
            position++;
            return true;
        }

        return false;
    }

    private void Require(char wanted)
    {
        if (!Match(wanted))
        {
            throw new ArgumentException("Missing bracket or invalid expression.");
        }
    }
}
