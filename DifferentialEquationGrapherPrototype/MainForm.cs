using System;
using System.Drawing;
using System.Windows.Forms;

namespace DifferentialEquationGrapherPrototype;

public class MainForm : Form
{
    private TextBox equationBox;
    private Button plotButton;
    private ComboBox modeBox;
    private NumericUpDown termsBox;
    private NumericUpDown xMinimumBox;
    private NumericUpDown xMaximumBox;
    private NumericUpDown yMinimumBox;
    private NumericUpDown yMaximumBox;
    private GraphPanel graphPanel;
    private Label statusLabel;
    private bool hasPlotted;

    public MainForm()
    {
        Text = "Function Grapher";
        Width = 1200;
        Height = 750;
        MinimumSize = new Size(900, 600);
        StartPosition = FormStartPosition.CenterScreen;

        equationBox = new TextBox();
        plotButton = new Button();
        modeBox = new ComboBox();
        termsBox = new NumericUpDown();
        xMinimumBox = new NumericUpDown();
        xMaximumBox = new NumericUpDown();
        yMinimumBox = new NumericUpDown();
        yMaximumBox = new NumericUpDown();
        graphPanel = new GraphPanel();
        statusLabel = new Label();
        hasPlotted = false;

        BuildInterface();
    }

    private void BuildInterface()
    {
        Panel topPanel = new Panel();
        topPanel.Dock = DockStyle.Top;
        topPanel.Height = 76;
        topPanel.Padding = new Padding(12);
        topPanel.BackColor = Color.FromArgb(245, 247, 250);

        Label equationLabel = new Label();
        equationLabel.Text = "f(x)";
        equationLabel.AutoSize = true;
        equationLabel.Location = new Point(12, 25);
        topPanel.Controls.Add(equationLabel);

        equationBox.Location = new Point(55, 19);
        equationBox.Width = 300;
        equationBox.Font = new Font("Segoe UI", 11);
        topPanel.Controls.Add(equationBox);

        plotButton.Text = "Plot";
        plotButton.Location = new Point(365, 17);
        plotButton.Size = new Size(75, 32);
        plotButton.Click += PlotButton_Click;
        topPanel.Controls.Add(plotButton);

        Label modeLabel = new Label();
        modeLabel.Text = "Mode";
        modeLabel.AutoSize = true;
        modeLabel.Location = new Point(465, 25);
        topPanel.Controls.Add(modeLabel);

        modeBox.DropDownStyle = ComboBoxStyle.DropDownList;
        modeBox.Items.Add("Standard");
        modeBox.Items.Add("Taylor");
        modeBox.SelectedIndex = 0;
        modeBox.Location = new Point(510, 19);
        modeBox.Width = 105;
        modeBox.SelectedIndexChanged += ModeBox_SelectedIndexChanged;
        topPanel.Controls.Add(modeBox);

        Label termsLabel = new Label();
        termsLabel.Text = "Terms";
        termsLabel.AutoSize = true;
        termsLabel.Location = new Point(640, 25);
        topPanel.Controls.Add(termsLabel);

        termsBox.Minimum = 1;
        termsBox.Maximum = 30;
        termsBox.Value = 8;
        termsBox.Increment = 1;
        termsBox.Location = new Point(690, 19);
        termsBox.Width = 65;
        termsBox.Enabled = false;
        termsBox.ValueChanged += TermsBox_ValueChanged;
        topPanel.Controls.Add(termsBox);

        Panel leftPanel = new Panel();
        leftPanel.Dock = DockStyle.Left;
        leftPanel.Width = 220;
        leftPanel.Padding = new Padding(14);
        leftPanel.BackColor = Color.White;

        Label rangeTitle = new Label();
        rangeTitle.Text = "Graph range";
        rangeTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        rangeTitle.Dock = DockStyle.Top;
        rangeTitle.Height = 30;
        leftPanel.Controls.Add(rangeTitle);

        SetRangeBox(xMinimumBox, -10);
        SetRangeBox(xMaximumBox, 10);
        SetRangeBox(yMinimumBox, -5);
        SetRangeBox(yMaximumBox, 5);

        TableLayoutPanel rangeTable = new TableLayoutPanel();
        rangeTable.Dock = DockStyle.Top;
        rangeTable.Height = 170;
        rangeTable.ColumnCount = 2;
        rangeTable.RowCount = 4;
        rangeTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        rangeTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));

        AddRangeRow(rangeTable, 0, "X min", xMinimumBox);
        AddRangeRow(rangeTable, 1, "X max", xMaximumBox);
        AddRangeRow(rangeTable, 2, "Y min", yMinimumBox);
        AddRangeRow(rangeTable, 3, "Y max", yMaximumBox);
        leftPanel.Controls.Add(rangeTable);

        xMinimumBox.ValueChanged += RangeBox_ValueChanged;
        xMaximumBox.ValueChanged += RangeBox_ValueChanged;
        yMinimumBox.ValueChanged += RangeBox_ValueChanged;
        yMaximumBox.ValueChanged += RangeBox_ValueChanged;

        graphPanel.Dock = DockStyle.Fill;
        graphPanel.SetRange(-10, 10, -5, 5);

        statusLabel.Dock = DockStyle.Bottom;
        statusLabel.Height = 27;
        statusLabel.Padding = new Padding(10, 5, 0, 0);
        statusLabel.ForeColor = Color.DimGray;
        statusLabel.Text = "Ready";

        Controls.Add(graphPanel);
        Controls.Add(leftPanel);
        Controls.Add(topPanel);
        Controls.Add(statusLabel);
    }

    private void SetRangeBox(NumericUpDown box, decimal value)
    {
        box.Minimum = -1000;
        box.Maximum = 1000;
        box.DecimalPlaces = 2;
        box.Increment = 0.5m;
        box.Value = value;
        box.Dock = DockStyle.Fill;
    }

    private void AddRangeRow(TableLayoutPanel table, int row, string labelText, Control control)
    {
        Label label = new Label();
        label.Text = labelText;
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleLeft;

        table.Controls.Add(label, 0, row);
        table.Controls.Add(control, 1, row);
    }

    private void PlotButton_Click(object? sender, EventArgs e)
    {
        Plot();
    }

    private void ModeBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        termsBox.Enabled = modeBox.SelectedIndex == 1;
        ReplotIfNeeded();
    }

    private void TermsBox_ValueChanged(object? sender, EventArgs e)
    {
        ReplotIfNeeded();
    }

    private void RangeBox_ValueChanged(object? sender, EventArgs e)
    {
        UpdateRange();
    }

    private void UpdateRange()
    {
        if (xMinimumBox.Value >= xMaximumBox.Value || yMinimumBox.Value >= yMaximumBox.Value)
        {
            statusLabel.Text = "Minimum values must be below maximum values.";
            return;
        }

        graphPanel.SetRange(
            (double)xMinimumBox.Value,
            (double)xMaximumBox.Value,
            (double)yMinimumBox.Value,
            (double)yMaximumBox.Value);
    }

    private void ReplotIfNeeded()
    {
        if (hasPlotted && !string.IsNullOrWhiteSpace(equationBox.Text))
        {
            Plot();
        }
    }

    private void Plot()
    {
        if (xMinimumBox.Value >= xMaximumBox.Value || yMinimumBox.Value >= yMaximumBox.Value)
        {
            MessageBox.Show("Each minimum must be smaller than its maximum.", "Graph range");
            return;
        }

        try
        {
            bool useTaylor = modeBox.SelectedIndex == 1;
            int terms = (int)termsBox.Value;

            ExpressionParser parser = new ExpressionParser(equationBox.Text, useTaylor, terms);
            parser.Evaluate(0);

            graphPanel.SetRange(
                (double)xMinimumBox.Value,
                (double)xMaximumBox.Value,
                (double)yMinimumBox.Value,
                (double)yMaximumBox.Value);

            graphPanel.SetFunction(parser.Evaluate);
            hasPlotted = true;
            statusLabel.Text = "Plotted";
        }
        catch (Exception error)
        {
            graphPanel.SetFunction(null);
            hasPlotted = false;
            statusLabel.Text = "Invalid function";

            MessageBox.Show(error.Message, "Equation error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
