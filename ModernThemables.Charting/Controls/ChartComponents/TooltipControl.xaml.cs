namespace ModernThemables.Charting.Controls.ChartComponents;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.Services;
using ModernThemables.Charting.ViewModels;

/// <summary>
/// Interaction logic for TooltipControl.xaml.
/// </summary>
public partial class TooltipControl : UserControl
{
    public static readonly DependencyProperty ShowPointIndicatorsProperty = DependencyProperty.Register(
        nameof(ShowPointIndicators),
        typeof(bool),
        typeof(TooltipControl),
        new UIPropertyMetadata(true));

    public static readonly DependencyProperty ShowTooltipProperty = DependencyProperty.Register(
        nameof(ShowTooltip),
        typeof(bool),
        typeof(TooltipControl),
        new UIPropertyMetadata(true));

    public static readonly DependencyProperty ForceCrosshairsProperty = DependencyProperty.Register(
        nameof(ForceCrosshairs),
        typeof(bool?),
        typeof(TooltipControl),
        new UIPropertyMetadata(null, OnSetForceCrosshairs));

    public static readonly DependencyProperty ForceTooltipProperty = DependencyProperty.Register(
        nameof(ForceTooltip),
        typeof(bool?),
        typeof(TooltipControl),
        new UIPropertyMetadata(null, OnSetForceTooltip));

    public static readonly DependencyProperty IsMouseOverThisProperty = DependencyProperty.Register(
        nameof(IsMouseOverThis),
        typeof(bool),
        typeof(TooltipControl),
        new UIPropertyMetadata(false));

    public static readonly DependencyProperty PointClickedProperty = DependencyProperty.Register(
        nameof(PointClicked),
        typeof(bool),
        typeof(TooltipControl),
        new UIPropertyMetadata(false));

    public static readonly DependencyProperty ForcePointIndicatorsProperty = DependencyProperty.Register(
        nameof(ForcePointIndicators),
        typeof(bool?),
        typeof(TooltipControl),
        new UIPropertyMetadata(null, OnSetForcePointIndicators));

    public static readonly DependencyProperty ShowCrosshairsProperty = DependencyProperty.Register(
        nameof(ShowCrosshairs),
        typeof(bool),
        typeof(TooltipControl),
        new UIPropertyMetadata(true));

    public static readonly DependencyProperty AllowSelectionProperty = DependencyProperty.Register(
        nameof(AllowSelection),
        typeof(bool),
        typeof(TooltipControl),
        new UIPropertyMetadata(true));

    public static readonly DependencyProperty ForceTooltipTopProperty = DependencyProperty.Register(
        nameof(ForceTooltipTop),
        typeof(bool),
        typeof(TooltipControl),
        new UIPropertyMetadata(false));

    public static readonly DependencyProperty TooltipOffsetXProperty = DependencyProperty.Register(
        nameof(TooltipOffsetX),
        typeof(double),
        typeof(TooltipControl),
        new UIPropertyMetadata(0d));

    public static readonly DependencyProperty TooltipOffsetYProperty = DependencyProperty.Register(
        nameof(TooltipOffsetY),
        typeof(double),
        typeof(TooltipControl),
        new UIPropertyMetadata(0d));

    public static readonly DependencyProperty TooltipOpacityProperty = DependencyProperty.Register(
        nameof(TooltipOpacity),
        typeof(double),
        typeof(TooltipControl),
        new PropertyMetadata(1d));

    public static readonly DependencyProperty InvertYProperty = DependencyProperty.Register(
        nameof(InvertY),
        typeof(bool),
        typeof(TooltipControl),
        new UIPropertyMetadata(false));

    public static readonly DependencyProperty TooltipPointsProperty = DependencyProperty.Register(
        nameof(TooltipPoints),
        typeof(ObservableCollection<TooltipViewModel>),
        typeof(TooltipControl),
        new PropertyMetadata(new ObservableCollection<TooltipViewModel>()));

    public static readonly DependencyProperty TooltipLocationProperty = DependencyProperty.Register(
        nameof(TooltipLocation),
        typeof(TooltipLocation),
        typeof(TooltipControl),
        new UIPropertyMetadata(TooltipLocation.Cursor, OnTooltipLocationSet));

    public static readonly DependencyProperty TooltipGetterFuncProperty = DependencyProperty.Register(
        nameof(TooltipGetterFunc),
        typeof(Func<Point, IEnumerable<TooltipViewModel>>),
        typeof(TooltipControl),
        new PropertyMetadata(null));

    public static readonly DependencyProperty IsTooltipByCursorProperty = DependencyProperty.Register(
        nameof(IsTooltipByCursor),
        typeof(bool),
        typeof(TooltipControl),
        new PropertyMetadata(true));

    public static readonly DependencyProperty IsUserSelectingRangeProperty = DependencyProperty.Register(
        nameof(IsUserSelectingRange),
        typeof(bool),
        typeof(TooltipControl),
        new PropertyMetadata(false));

    public static readonly DependencyProperty MouseCoordinatorProperty = DependencyProperty.Register(
        nameof(MouseCoordinator),
        typeof(MouseCoordinator),
        typeof(TooltipControl),
        new PropertyMetadata(null, OnSetMouseCoordinator));

    private bool tooltipLeft;
    private bool tooltipTop;

    private bool isUserPanning;

    public TooltipControl()
    {
        this.InitializeComponent();

        this.Loaded += this.TooltipControl_Loaded;

        NameScope.SetNameScope(this.CM, NameScope.GetNameScope(this));
    }

    public bool? ForceCrosshairs
    {
        get => (bool?)this.GetValue(ForceCrosshairsProperty);
        set => this.SetValue(ForceCrosshairsProperty, value);
    }

    public bool? ForceTooltip
    {
        get => (bool?)this.GetValue(ForceTooltipProperty);
        set => this.SetValue(ForceTooltipProperty, value);
    }

    public bool IsMouseOverThis
    {
        get => (bool)this.GetValue(IsMouseOverThisProperty);
        private set => this.SetValue(IsMouseOverThisProperty, value);
    }

    public bool PointClicked
    {
        get => (bool)this.GetValue(PointClickedProperty);
        private set => this.SetValue(PointClickedProperty, value);
    }

    public bool? ForcePointIndicators
    {
        get => (bool?)this.GetValue(ForcePointIndicatorsProperty);
        set => this.SetValue(ForcePointIndicatorsProperty, value);
    }

    public bool AllowSelection
    {
        get => (bool)this.GetValue(AllowSelectionProperty);
        set => this.SetValue(AllowSelectionProperty, value);
    }

    public bool ForceTooltipTop
    {
        get => (bool)this.GetValue(ForceTooltipTopProperty);
        set => this.SetValue(ForceTooltipTopProperty, value);
    }

    public double TooltipOffsetX
    {
        get => (double)this.GetValue(TooltipOffsetXProperty);
        set => this.SetValue(TooltipOffsetXProperty, value);
    }

    public double TooltipOffsetY
    {
        get => (double)this.GetValue(TooltipOffsetYProperty);
        set => this.SetValue(TooltipOffsetYProperty, value);
    }

    public double TooltipOpacity
    {
        get => (double)this.GetValue(TooltipOpacityProperty);
        set => this.SetValue(TooltipOpacityProperty, value);
    }

    public bool InvertY
    {
        get => (bool)this.GetValue(InvertYProperty);
        set => this.SetValue(InvertYProperty, value);
    }

    public TooltipLocation TooltipLocation
    {
        get => (TooltipLocation)this.GetValue(TooltipLocationProperty);
        set => this.SetValue(TooltipLocationProperty, value);
    }

    public Func<Point, IEnumerable<TooltipViewModel>> TooltipGetterFunc
    {
        get => (Func<Point, IEnumerable<TooltipViewModel>>)this.GetValue(TooltipGetterFuncProperty);
        set => this.SetValue(TooltipGetterFuncProperty, value);
    }

    public MouseCoordinator Coordinator
    {
        get => (MouseCoordinator)this.GetValue(MouseCoordinatorProperty);
        set => this.SetValue(MouseCoordinatorProperty, value);
    }

    private bool ShowTooltip
    {
        get => (bool)this.GetValue(ShowTooltipProperty);
        set => this.SetValue(ShowTooltipProperty, value);
    }

    private bool ShowCrosshairs
    {
        get => (bool)this.GetValue(ShowCrosshairsProperty);
        set => this.SetValue(ShowCrosshairsProperty, value);
    }

    private ObservableCollection<TooltipViewModel> TooltipPoints
    {
        get => (ObservableCollection<TooltipViewModel>)this.GetValue(TooltipPointsProperty);
        set => this.SetValue(TooltipPointsProperty, value);
    }

    private bool IsTooltipByCursor
    {
        get => (bool)this.GetValue(IsTooltipByCursorProperty);
        set => this.SetValue(IsTooltipByCursorProperty, value);
    }

    private bool IsUserSelectingRange
    {
        get => (bool)this.GetValue(IsUserSelectingRangeProperty);
        set => this.SetValue(IsUserSelectingRangeProperty, value);
    }

    private bool ShowPointIndicators
    {
        get => (bool)this.GetValue(ShowPointIndicatorsProperty);
        set => this.SetValue(ShowPointIndicatorsProperty, value);
    }

    private static void OnSetMouseCoordinator(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not TooltipControl control)
        {
            return;
        }

        control.Coordinator.MouseMove += control.Coordinator_MouseMove;
        control.Coordinator.MouseLeave += control.Coordinator_MouseLeave;
        control.Coordinator.PointClicked += control.Coordinator_PointClicked;
        control.Coordinator.PointRangeSelected += control.Coordinator_PointRangeSelected;
        control.Coordinator.PreviewMouseUp += control.Coordinator_PreviewMouseUp;
    }

    private static void OnTooltipLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not TooltipControl control)
        {
            return;
        }

        control.IsTooltipByCursor = control.TooltipLocation == TooltipLocation.Cursor;
    }

    private static void OnSetForceTooltip(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not TooltipControl control)
        {
            return;
        }

        if (control.ForceTooltip.HasValue && !control.ForceTooltip.Value)
        {
            control.ShowTooltip = false;
        }
    }

    private static void OnSetForcePointIndicators(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not TooltipControl control)
        {
            return;
        }

        if (control.ForcePointIndicators.HasValue && !control.ForcePointIndicators.Value)
        {
            control.ShowPointIndicators = false;
        }
    }

    private static void OnSetForceCrosshairs(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not TooltipControl control)
        {
            return;
        }

        if (control.ForceCrosshairs.HasValue && !control.ForceCrosshairs.Value)
        {
            control.ShowCrosshairs = false;
        }
    }

    private void TooltipControl_Loaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= this.TooltipControl_Loaded;
        if (ChartHelper.FindMouseCoordinatorFromVisualTree(this, out var coordinator))
        {
            this.Coordinator = coordinator!;
        }
        else
        {
#if !DEBUG
            throw new InvalidOperationException("Please add a MouseCoordinator to your chart");
#endif
        }
    }

    private void Coordinator_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Right && !this.isUserPanning)
        {
            this.CM.IsOpen = true;
        }
    }

    private void Coordinator_PointRangeSelected(object? sender, (Point LowerValue, Point UpperValue) e)
        => this.IsUserSelectingRange = false;

    private void Coordinator_PointClicked(object? sender, Point e)
    {
        this.PointClicked = true;
        this.PointClicked = false;
    }

    private void Coordinator_MouseLeave(object sender, MouseEventArgs e)
        => this.IsMouseOverThis = false;

    private void Coordinator_MouseMove(object? sender, MouseCoordinatorMouseMoveEventArgs e)
    {
        this.IsMouseOverThis = true;
        var mouseLoc = e.Args.GetPosition(this.Grid);
        this.isUserPanning = e.IsUserPanning;

        if (this.isUserPanning)
        {
            this.IsMouseOverThis = false;
            return;
        }

        // Move crosshairs
        if (this.ShowCrosshairs)
        {
            this.XCrosshair.Margin = new Thickness(0, mouseLoc.Y, 0, 0);
            this.YCrosshair.Margin = new Thickness(mouseLoc.X, 0, 0, 0);
        }

        // Adjust selected range
        if (this.AllowSelection && e.IsUserDragging && e.LowerSelection != null)
        {
            this.IsUserSelectingRange = true;
            var negative = mouseLoc.X < e.LowerSelection.Value.X;
            var margin = this.SelectionRangeBorder.Margin;
            margin.Left = negative ? mouseLoc.X : e.LowerSelection.Value.X;
            this.SelectionRangeBorder.Margin = margin;
            this.SelectionRangeBorder.Width = negative
                ? Math.Max(e.LowerSelection.Value.X - mouseLoc.X, 0)
                : Math.Max(mouseLoc.X - e.LowerSelection.Value.X, 0);
        }
        else
        {
            this.IsUserSelectingRange = false;
        }

        // Place/populate tooltip
        if ((this.ShowTooltip || this.ShowPointIndicators) && this.TooltipGetterFunc != null)
        {
            this.TooltipPoints = new ObservableCollection<TooltipViewModel>(this.TooltipGetterFunc(mouseLoc));

            if (this.TooltipPoints.Any())
            {
                this.TooltipPoints.First(x => x.LocationY - mouseLoc.Y == this.TooltipPoints.Min(y => y.LocationY - mouseLoc.Y)).IsNearest = true;

                foreach (var point in this.TooltipPoints)
                {
                    point.ResizeTrigger = true;
                }

                if (this.ShowTooltip && this.TooltipLocation == TooltipLocation.Cursor)
                {
                    // Get tooltip position variables
                    if (!this.tooltipLeft && (this.ActualWidth - mouseLoc.X) < (this.TooltipsByCursor.ActualWidth + 10))
                    {
                        this.tooltipLeft = true;
                    }

                    if (this.tooltipLeft && mouseLoc.X < (this.TooltipsByCursor.ActualWidth + 5))
                    {
                        this.tooltipLeft = false;
                    }

                    if (!this.tooltipTop && (this.ActualHeight - mouseLoc.Y) < (this.TooltipsByCursor.ActualHeight + 10))
                    {
                        this.tooltipTop = true;
                    }

                    if (this.tooltipTop && mouseLoc.Y < (this.TooltipsByCursor.ActualHeight + 5))
                    {
                        this.tooltipTop = false;
                    }

                    this.TooltipsByCursor.Margin = new Thickness(
                        !this.tooltipLeft ? mouseLoc.X + 5 : mouseLoc.X - this.TooltipsByCursor.ActualWidth - 5,
                        !this.tooltipTop ? mouseLoc.Y + 5 : mouseLoc.Y - this.TooltipsByCursor.ActualHeight - 5,
                        0,
                        0);
                }
            }
        }
        else
        {
            this.TooltipPoints.Clear();
        }
    }
}
