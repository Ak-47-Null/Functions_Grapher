using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DifferentialEquationGrapherPrototype;

public class GraphPanel : Panel
{
    private Func<double, double>? function;
    private GraphRange range;
    private GraphRenderer renderer;

    public GraphPanel()
    {
        range = new GraphRange(-10, 10, -5, 5);
        renderer = new GraphRenderer();

        DoubleBuffered = true;
        ResizeRedraw = true;

        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
    }

    public void SetFunction(Func<double, double>? newFunction)
    {
        function = newFunction;
        Invalidate();
    }

    public void SetRange(double xMinimum, double xMaximum, double yMinimum, double yMaximum)
    {
        GraphRange newRange = new GraphRange(xMinimum, xMaximum, yMinimum, yMaximum);

        if (!newRange.IsValid())
        {
            return;
        }

        range = newRange;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Color.White);

        if (Width < 50 || Height < 50)
        {
            return;
        }

        renderer.Draw(e.Graphics, function, range, Width, Height);
    }
}
