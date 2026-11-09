using System;
using System.Collections.Generic;
using System.Drawing;

namespace DifferentialEquationGrapherPrototype;

public class GraphRenderer
{
    private GraphSampler sampler = new GraphSampler();

    public void Draw(Graphics graphics, Func<double, double>? function, GraphRange range, int width, int height)
    {
        DrawGrid(graphics, range, width, height);
        DrawAxes(graphics, range, width, height);

        if (function != null)
        {
            DrawFunction(graphics, function, range, width, height);
        }
    }

    private void DrawGrid(Graphics graphics, GraphRange range, int width, int height)
    {
        using (Pen gridPen = new Pen(Color.FromArgb(230, 233, 238)))
        {
            double xStep = NiceStep((range.XMaximum - range.XMinimum) / 10);
            double yStep = NiceStep((range.YMaximum - range.YMinimum) / 8);

            double firstX = Math.Ceiling(range.XMinimum / xStep) * xStep;

            for (double x = firstX; x <= range.XMaximum; x = x + xStep)
            {
                float screenX = range.ConvertX(x, width);
                graphics.DrawLine(gridPen, screenX, 0, screenX, height);
            }

            double firstY = Math.Ceiling(range.YMinimum / yStep) * yStep;

            for (double y = firstY; y <= range.YMaximum; y = y + yStep)
            {
                float screenY = range.ConvertY(y, height);
                graphics.DrawLine(gridPen, 0, screenY, width, screenY);
            }
        }
    }

    private void DrawAxes(Graphics graphics, GraphRange range, int width, int height)
    {
        using (Pen axisPen = new Pen(Color.FromArgb(70, 70, 75), 1.5f))
        {
            if (range.XMinimum <= 0 && range.XMaximum >= 0)
            {
                float x = range.ConvertX(0, width);
                graphics.DrawLine(axisPen, x, 0, x, height);
            }

            if (range.YMinimum <= 0 && range.YMaximum >= 0)
            {
                float y = range.ConvertY(0, height);
                graphics.DrawLine(axisPen, 0, y, width, y);
            }
        }
    }

    private void DrawFunction(Graphics graphics, Func<double, double> function, GraphRange range, int width, int height)
    {
        List<List<PointF>> segments = sampler.GetSegments(function, range, width, height);

        using (Pen functionPen = new Pen(Color.FromArgb(35, 95, 205), 2.2f))
        {
            foreach (List<PointF> segment in segments)
            {
                if (segment.Count > 1)
                {
                    graphics.DrawLines(functionPen, segment.ToArray());
                }
            }
        }
    }

    private double NiceStep(double value)
    {
        double fraction = value;
        double scale = 1;

        while (fraction >= 10)
        {
            fraction = fraction / 10;
            scale = scale * 10;
        }

        while (fraction < 1)
        {
            fraction = fraction * 10;
            scale = scale / 10;
        }

        double niceNumber;

        if (fraction <= 1)
        {
            niceNumber = 1;
        }
        else if (fraction <= 2)
        {
            niceNumber = 2;
        }
        else if (fraction <= 5)
        {
            niceNumber = 5;
        }
        else
        {
            niceNumber = 10;
        }

        return niceNumber * scale;
    }
}
