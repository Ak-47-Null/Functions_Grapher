using System;

namespace DifferentialEquationGrapherPrototype;

public class MathFunctions
{
    private TaylorSeries taylor = new TaylorSeries();

    public double Apply(string name, double value, bool useTaylor, int terms)
    {
        if (name == "sin")
        {
            if (useTaylor) return taylor.Sin(value, terms);
            return Math.Sin(value);
        }

        if (name == "cos")
        {
            if (useTaylor) return taylor.Cos(value, terms);
            return Math.Cos(value);
        }

        if (name == "tan")
        {
            if (useTaylor) return taylor.Tan(value, terms);
            return Math.Tan(value);
        }

        if (name == "asin") return Math.Asin(value);
        if (name == "acos") return Math.Acos(value);
        if (name == "atan") return Math.Atan(value);
        if (name == "sqrt") return Math.Sqrt(value);
        if (name == "abs") return Math.Abs(value);
        if (name == "ln") return Math.Log(value);
        if (name == "log") return Math.Log10(value);
        if (name == "exp") return Math.Exp(value);

        throw new ArgumentException("Unknown function: " + name);
    }

    public double Power(double a, double b)
    {
        double rounded = Math.Round(b);

        if (Math.Abs(b - rounded) < 0.0000000001 && Math.Abs(rounded) < 10000)
        {
            int n = (int)rounded;

            if (n == 0)
            {
                return 1;
            }

            bool negative = n < 0;

            if (negative)
            {
                n = -n;
            }

            double result = 1;

            for (int i = 0; i < n; i++)
            {
                result = result * a;
            }

            if (negative)
            {
                return 1 / result;
            }

            return result;
        }

        if (a <= 0)
        {
            return double.NaN;
        }

        return Math.Exp(b * Math.Log(a));
    }
}
