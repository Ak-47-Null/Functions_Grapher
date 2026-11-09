using System;
using System.Collections.Generic;
using System.Drawing;

namespace DifferentialEquationGrapherPrototype;

public class GraphSampler
{
    private const int SampleCount = 2400;

    public List<List<PointF>> GetSegments(Func<double, double> function, GraphRange range, int width, int height)
    {
        List<List<PointF>> segments = new List<List<PointF>>();
        List<PointF> currentSegment = new List<PointF>();
        double visibleMargin = 10 * (range.YMaximum - range.YMinimum);

        for (int i = 0; i <= SampleCount; i++)
        {
            double x = range.XMinimum + (range.XMaximum - range.XMinimum) * i / SampleCount;

            try
            {
                double y = function(x);

                if (double.IsNaN(y) || double.IsInfinity(y))
                {
                    AddSegment(segments, currentSegment);
                    currentSegment.Clear();
                    continue;
                }

                if (y < range.YMinimum - visibleMargin || y > range.YMaximum + visibleMargin)
                {
                    AddSegment(segments, currentSegment);
                    currentSegment.Clear();
                    continue;
                }

                float screenX = range.ConvertX(x, width);
                float screenY = range.ConvertY(y, height);
                PointF point = new PointF(screenX, screenY);

                if (currentSegment.Count > 0)
                {
                    PointF previousPoint = currentSegment[currentSegment.Count - 1];
                    double difference = Math.Abs(point.Y - previousPoint.Y);

                    if (difference > height * 0.75)
                    {
                        AddSegment(segments, currentSegment);
                        currentSegment.Clear();
                    }
                }

                currentSegment.Add(point);
            }
            catch
            {
                AddSegment(segments, currentSegment);
                currentSegment.Clear();
            }
        }

        AddSegment(segments, currentSegment);
        return segments;
    }

    private void AddSegment(List<List<PointF>> segments, List<PointF> points)
    {
        if (points.Count > 1)
        {
            List<PointF> newSegment = new List<PointF>(points);
            segments.Add(newSegment);
        }
    }
}
