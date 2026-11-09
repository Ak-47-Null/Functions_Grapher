using System;

namespace DifferentialEquationGrapherPrototype;

public class TaylorSeries
{
    public double Sin(double x, int terms)
    {
        double total = 0;
        double power = x;
        double factorial = 1;

        for (int i = 0; i < terms; i++)
        {
            if (i > 0)
            {
                power = power * x * x;
                factorial = factorial * (2 * i) * (2 * i + 1);
            }

            double term = power / factorial;

            if (i % 2 == 0)
            {
                total = total + term;
            }
            else
            {
                total = total - term;
            }
        }

        return total;
    }

    public double Cos(double x, int terms)
    {
        double total = 0;
        double power = 1;
        double factorial = 1;

        for (int i = 0; i < terms; i++)
        {
            if (i > 0)
            {
                power = power * x * x;
                factorial = factorial * (2 * i - 1) * (2 * i);
            }

            double term = power / factorial;

            if (i % 2 == 0)
            {
                total = total + term;
            }
            else
            {
                total = total - term;
            }
        }

        return total;
    }

    public double Tan(double x, int terms)
    {
        double sine = Sin(x, terms);
        double cosine = Cos(x, terms);

        if (Math.Abs(cosine) < 0.0000001)
        {
            return double.NaN;
        }

        return sine / cosine;
    }
}
