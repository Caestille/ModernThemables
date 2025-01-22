namespace ModernThemables.Charting.Controls.ChartComponents;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CoreUtilities.Converters;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.Services;

/// <summary>
/// Interaction logic for AxisControl.xaml.
/// </summary>
public partial class AxisControl : UserControl
{
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(AxisControl),
        new UIPropertyMetadata(Orientation.Vertical, OnSetAxisOrientation));

    public static readonly DependencyProperty LabelRotationProperty = DependencyProperty.Register(
        nameof(LabelRotation),
        typeof(double),
        typeof(AxisControl),
        new UIPropertyMetadata(0d, OnSetLabelRotation));

    public static readonly DependencyProperty ShowDividersProperty = DependencyProperty.Register(
        nameof(ShowDividers),
        typeof(bool),
        typeof(AxisControl),
        new UIPropertyMetadata(true));

    public static readonly DependencyProperty ShowIndicatorsProperty = DependencyProperty.Register(
        nameof(ShowIndicators),
        typeof(bool),
        typeof(AxisControl),
        new UIPropertyMetadata(true));

    public static readonly DependencyProperty DividerWidthtProperty = DependencyProperty.Register(
        nameof(DividerWidth),
        typeof(double),
        typeof(AxisControl),
        new UIPropertyMetadata(0d));

    public static readonly DependencyProperty DividerHeightProperty = DependencyProperty.Register(
        nameof(DividerHeight),
        typeof(double),
        typeof(AxisControl),
        new UIPropertyMetadata(0d));

    public static readonly DependencyProperty DividerOffsetProperty = DependencyProperty.Register(
        nameof(DividerOffset),
        typeof(double),
        typeof(AxisControl),
        new UIPropertyMetadata(0d));

    public static readonly DependencyProperty DividerBorderThicknessProperty = DependencyProperty.Register(
        nameof(DividerBorderThickness),
        typeof(Thickness),
        typeof(AxisControl),
        new UIPropertyMetadata(new Thickness(0)));

    public static readonly DependencyProperty MarginStringProperty = DependencyProperty.Register(
        nameof(MarginString),
        typeof(string),
        typeof(AxisControl),
        new UIPropertyMetadata("0-0-0-1"));

    public static readonly DependencyProperty DividerAlignmentProperty = DependencyProperty.Register(
        nameof(DividerAlignment),
        typeof(HorizontalAlignment),
        typeof(AxisControl),
        new UIPropertyMetadata(HorizontalAlignment.Right));

    public static readonly DependencyProperty AlignmentProperty = DependencyProperty.Register(
        nameof(Alignment),
        typeof(VerticalAlignment),
        typeof(AxisControl),
        new UIPropertyMetadata(VerticalAlignment.Bottom));

    public static readonly DependencyProperty LabelsProperty = DependencyProperty.Register(
        nameof(Labels),
        typeof(ObservableCollection<AxisLabel>),
        typeof(AxisControl),
        new UIPropertyMetadata(null, OnSetLabelRotation));

    public static readonly DependencyProperty MouseCoordinatorProperty = DependencyProperty.Register(
        nameof(MouseCoordinator),
        typeof(MouseCoordinator),
        typeof(AxisControl),
        new PropertyMetadata(null, OnSetMouseCoordinator));

    public AxisControl()
    {
        this.InitializeComponent();
        this.MainItemsControl.SizeChanged += this.MainItemsControl_SizeChanged;
        this.Loaded += this.AxisControl_Loaded;
    }

    public Orientation Orientation
    {
        get => (Orientation)this.GetValue(OrientationProperty);
        set => this.SetValue(OrientationProperty, value);
    }

    public double LabelRotation
    {
        get => (double)this.GetValue(LabelRotationProperty);
        set => this.SetValue(LabelRotationProperty, value);
    }

    public bool ShowDividers
    {
        get => (bool)this.GetValue(ShowDividersProperty);
        set => this.SetValue(ShowDividersProperty, value);
    }

    public bool ShowIndicators
    {
        get => (bool)this.GetValue(ShowIndicatorsProperty);
        set => this.SetValue(ShowIndicatorsProperty, value);
    }

    public double DividerOffset
    {
        get => (double)this.GetValue(DividerOffsetProperty);
        set => this.SetValue(DividerOffsetProperty, value);
    }

    public ObservableCollection<AxisLabel> Labels
    {
        get => (ObservableCollection<AxisLabel>)this.GetValue(LabelsProperty);
        set => this.SetValue(LabelsProperty, value);
    }

    public MouseCoordinator Coordinator
    {
        get => (MouseCoordinator)this.GetValue(MouseCoordinatorProperty);
        set => this.SetValue(MouseCoordinatorProperty, value);
    }

    private double DividerWidth
    {
        get => (double)this.GetValue(DividerWidthtProperty);
        set => this.SetValue(DividerWidthtProperty, value);
    }

    private double DividerHeight
    {
        get => (double)this.GetValue(DividerHeightProperty);
        set => this.SetValue(DividerHeightProperty, value);
    }

    private Thickness DividerBorderThickness
    {
        get => (Thickness)this.GetValue(DividerBorderThicknessProperty);
        set => this.SetValue(DividerBorderThicknessProperty, value);
    }

    private string MarginString
    {
        get => (string)this.GetValue(MarginStringProperty);
        set => this.SetValue(MarginStringProperty, value);
    }

    private HorizontalAlignment DividerAlignment
    {
        get => (HorizontalAlignment)this.GetValue(DividerAlignmentProperty);
        set => this.SetValue(DividerAlignmentProperty, value);
    }

    private VerticalAlignment Alignment
    {
        get => (VerticalAlignment)this.GetValue(AlignmentProperty);
        set => this.SetValue(AlignmentProperty, value);
    }

    private static void OnSetAxisOrientation(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not AxisControl axisControl)
        {
            return;
        }

        switch (axisControl.Orientation)
        {
            case Orientation.Vertical:
                axisControl.MarginString = "0-0-0-1";
                break;
            case Orientation.Horizontal:
                axisControl.MarginString = "1-0-0-0";
                break;
        }

        OnSetLabelRotation(sender, e);
    }

    private static void OnSetMouseCoordinator(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not AxisControl axisControl)
        {
            return;
        }

        axisControl.Coordinator.MouseMove += axisControl.Coordinator_MouseMove;
        axisControl.Coordinator.MouseLeave += axisControl.Coordinator_MouseLeave;
        axisControl.Coordinator.MouseEnter += axisControl.Coordinator_MouseEnter;
    }

    private static void OnSetLabelRotation(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not AxisControl axisControl || axisControl.Labels == null || !axisControl.Labels.Any())
        {
            return;
        }

        var sizes = axisControl.Labels.Select(
            x => StringWidthGetterConverter.MeasureString(
                x.FormattedValue,
                axisControl.FontSize,
                axisControl.FontFamily,
                axisControl.FontStyle,
                axisControl.FontWeight,
                axisControl.FontStretch));

        var width = sizes.Max(x => x.Width);
        var height = sizes.Max(x => x.Height);

        var mult = axisControl.Orientation == Orientation.Vertical
            ? Math.Cos(axisControl.LabelRotation * Math.PI / 180)
            : Math.Sin(axisControl.LabelRotation * Math.PI / 180);
        var value2 = (width * mult) + (axisControl.Orientation == Orientation.Horizontal ? 20 : 10);

        switch (axisControl.Orientation)
        {
            case Orientation.Horizontal:
                axisControl.MainItemsControl.Height = axisControl.DividerItemsControl.Height = value2;
                axisControl.MainItemsControl.Width = axisControl.DividerItemsControl.Width = double.NaN;
                axisControl.MainItemsControl.Margin = axisControl.DividerItemsControl.Margin = new Thickness(-axisControl.BorderThickness.Left, 0, 0, 0);
                axisControl.Alignment = VerticalAlignment.Top;
                axisControl.DividerWidth = 2;
                axisControl.DividerHeight = 6;
                axisControl.DividerBorderThickness = new Thickness(1, 0, 0, 0);
                axisControl.DividerAlignment = HorizontalAlignment.Left;
                break;
            case Orientation.Vertical:
                axisControl.MainItemsControl.Width = axisControl.DividerItemsControl.Width = value2;
                axisControl.MainItemsControl.Height = axisControl.DividerItemsControl.Height = double.NaN;
                axisControl.MainItemsControl.Margin = new Thickness(-axisControl.BorderThickness.Left, 0, 0, -height / 2);
                axisControl.DividerItemsControl.Margin = new Thickness(-axisControl.BorderThickness.Left, 0, 0, -1);
                axisControl.Alignment = VerticalAlignment.Bottom;
                axisControl.DividerWidth = 6;
                axisControl.DividerHeight = 2;
                axisControl.DividerBorderThickness = new Thickness(0, 0, 0, 1);
                axisControl.DividerAlignment = HorizontalAlignment.Right;
                break;
        }
    }

    private void AxisControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (ChartHelper.FindMouseCoordinatorFromVisualTree(this, out var coordinator))
        {
            this.Coordinator = coordinator!;
        }

        this.Loaded -= this.AxisControl_Loaded;
    }

    private void MainItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        => OnSetLabelRotation(this, default);

    private void Coordinator_MouseMove(object? sender, MouseCoordinatorMouseMoveEventArgs e)
    {
        if (!this.ShowIndicators)
        {
            return;
        }

        var mouseLoc = e.Args.GetPosition(this.Coordinator);
        var axisLength = this.Orientation == Orientation.Horizontal ? this.Grid.ActualWidth : this.Grid.ActualHeight;
        var axisFrac = this.Orientation == Orientation.Horizontal
            ? mouseLoc.X / axisLength
            : 1 - (mouseLoc.Y / axisLength);
        AxisLabel? labelMin = this.Labels.FirstOrDefault(x => x.Location == this.Labels.Min(y => y.Location));
        var minFrac = (labelMin?.Location ?? 0) / axisLength;
        AxisLabel? labelMax = this.Labels.FirstOrDefault(x => x.Location == this.Labels.Max(y => y.Location));
        var maxFrac = (labelMax?.Location ?? 0) / axisLength;
        var fullRange = (labelMax?.Value - labelMin?.Value) / (maxFrac - minFrac);

        if (fullRange != null && double.IsNaN(fullRange.Value))
        {
            return;
        }

        var min = labelMin?.Value - (minFrac * fullRange);
        var max = labelMax?.Value + ((1 - maxFrac) * fullRange);

        var value = min + (axisFrac * (max - min));

        this.ValueLabel.Text = (this.Labels.First().IndicatorFormatter ?? this.Labels.First().ValueFormatter)(value ?? 0);
        this.ValueDisplay.Margin = this.Orientation == Orientation.Horizontal
            ? new Thickness((axisFrac * this.Grid.ActualWidth) - (this.ValueDisplay.ActualWidth / 2), 4, -100, -100)
            : new Thickness(-5, ((1 - axisFrac) * this.Grid.ActualHeight) - 9, -100, 0);
    }

    private void Coordinator_MouseLeave(object sender, MouseEventArgs e)
    {
        if (this.ValueDisplay.Visibility == Visibility.Visible)
        {
            this.ValueDisplay.Visibility = Visibility.Collapsed;
        }
    }

    private void Coordinator_MouseEnter(object sender, MouseEventArgs e)
    {
        if (this.ShowIndicators && this.ValueDisplay.Visibility == Visibility.Collapsed)
        {
            this.ValueDisplay.Visibility = Visibility.Visible;
        }
    }
}
