using System;

namespace DifferentialEquationGrapherPrototype;

public class GraphRange
{
    public double XMinimum { get; private set; }
    public double XMaximum { get; private set; }
    public double YMinimum { get; private set; }
    public double YMaximum { get; private set; }

    public GraphRange(double xMinimum, double xMaximum, double yMinimum, double yMaximum)
    {
        XMinimum = xMinimum;
        XMaximum = xMaximum;
        YMinimum = yMinimum;
        YMaximum = yMaximum;
    }

    public bool IsValid()
    {
        if (XMinimum >= XMaximum)
        {
            return false;
        }

        if (YMinimum >= YMaximum)
        {
            return false;
        }

        return true;
    }

    public float ConvertX(double x, int width)
    {
        double screenX = (x - XMinimum) / (XMaximum - XMinimum) * width;
        return (float)screenX;
    }

    public float ConvertY(double y, int height)
    {
        double screenY = height - (y - YMinimum) / (YMaximum - YMinimum) * height;
        return (float)screenY;
    }
}
