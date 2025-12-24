namespace ModernThemables.Controls;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

[TemplatePart(Name = PARTMidRange, Type = typeof(RepeatButton))]
[TemplatePart(Name = PARTHigherSlider, Type = typeof(Slider))]
[TemplatePart(Name = PARTLowerSlider, Type = typeof(Slider))]
[TemplatePart(Name = PARTTrack, Type = typeof(Track))]

public class RangeSlider : Slider2
{
    public static readonly RoutedEvent LowerValueChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(LowerValueChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(RangeSlider));

    public static readonly RoutedEvent HigherValueChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(HigherValueChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(RangeSlider));

    /// <summary>
    /// HigherValue property represents the higher value within the selected range.
    /// </summary>
    public static readonly DependencyProperty HigherValueProperty = DependencyProperty.Register(
        nameof(HigherValue),
        typeof(double),
        typeof(RangeSlider),
        new FrameworkPropertyMetadata(
           0d,
           FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
           OnHigherValueChanged,
           OnCoerceHigherValueChanged));

    /// <summary>
    /// LowerValue property represents the lower value within the selected range.
    /// </summary>
    public static readonly DependencyProperty LowerValueProperty = DependencyProperty.Register(
        nameof(LowerValue),
        typeof(double),
        typeof(RangeSlider),
        new FrameworkPropertyMetadata(
           0d,
           FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
           OnLowerValueChanged,
           OnCoerceLowerValueChanged));

    /// <summary>
    /// RangeWidth property is a readonly property, used to calculate the percentage of the range within the entire min/max range.
    /// </summary>
    private static readonly DependencyPropertyKey RangeWidthPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
        nameof(RangeWidth),
        typeof(double),
        typeof(RangeSlider),
        new PropertyMetadata(0d));

    /// <summary>
    /// RangeMargin property is a readonly property, used to calculate the offset of the range within the left hand side of the range.
    /// </summary>
    private static readonly DependencyPropertyKey RangeMarginPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
        nameof(RangeMargin),
        typeof(Thickness),
        typeof(RangeSlider),
        new PropertyMetadata(new Thickness(0)));

#pragma warning disable SA1202 // Elements should be ordered by access
    public static readonly DependencyProperty RangeMarginProperty = RangeMarginPropertyKey.DependencyProperty;

    public static readonly DependencyProperty RangeWidthProperty = RangeWidthPropertyKey.DependencyProperty;
#pragma warning restore SA1202 // Elements should be ordered by access

    private const string PARTMidRange = "PART_MidRange";
    private const string PARTHigherSlider = "PART_HigherSlider";
    private const string PARTLowerSlider = "PART_LowerSlider";
    private const string PARTTrack = "PART_Track";

    private bool midRangeMouseDown;
    private Point midRangeMouseDownPoint;

    private RepeatButton? midRange;
    private Slider? lowerSlider;
    private Slider? higherSlider;

    static RangeSlider()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(typeof(RangeSlider)));
    }

    public RangeSlider()
    {
        this.SizeChanged += this.RangeSlider_SizeChanged;
    }

    public event RoutedEventHandler LowerValueChanged
    {
        add => this.AddHandler(RangeSlider.LowerValueChangedEvent, value);
        remove => this.RemoveHandler(RangeSlider.LowerValueChangedEvent, value);
    }

    public event RoutedEventHandler HigherValueChanged
    {
        add => this.AddHandler(RangeSlider.HigherValueChangedEvent, value);
        remove => this.RemoveHandler(RangeSlider.HigherValueChangedEvent, value);
    }

    public double HigherValue
    {
        get => (double)this.GetValue(RangeSlider.HigherValueProperty);
        set => this.SetValue(RangeSlider.HigherValueProperty, value);
    }

    public double LowerValue
    {
        get => (double)this.GetValue(RangeSlider.LowerValueProperty);
        set => this.SetValue(RangeSlider.LowerValueProperty, value);
    }

    public double RangeWidth
    {
        get => (double)this.GetValue(RangeSlider.RangeWidthProperty);
        private set => this.SetValue(RangeSlider.RangeWidthPropertyKey, value);
    }

    public Thickness RangeMargin
    {
        get => (Thickness)this.GetValue(RangeSlider.RangeMarginProperty);
        private set => this.SetValue(RangeSlider.RangeMarginPropertyKey, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (this.midRange != null)
        {
            this.midRange.PreviewMouseDown += this.MidRange_MouseDown;
            this.midRange.PreviewMouseMove -= this.MidRange_MouseMove;
            this.midRange.PreviewMouseUp -= this.MidRange_MouseUp;
        }

        this.midRange = this.Template.FindName(PARTMidRange, this) as RepeatButton;
        if (this.midRange != null)
        {
            this.midRange.PreviewMouseDown += this.MidRange_MouseDown;
            this.midRange.PreviewMouseMove += this.MidRange_MouseMove;
            this.midRange.PreviewMouseUp += this.MidRange_MouseUp;
        }

        if (this.lowerSlider != null)
        {
            this.lowerSlider.Loaded -= this.Slider_Loaded;
            this.lowerSlider.ValueChanged -= this.LowerSlider_ValueChanged;
        }

        this.lowerSlider = this.Template.FindName(PARTLowerSlider, this) as Slider;
        if (this.lowerSlider != null)
        {
            this.lowerSlider.Loaded += this.Slider_Loaded;
            this.lowerSlider.ValueChanged += this.LowerSlider_ValueChanged;
            this.lowerSlider.ApplyTemplate();
        }

        if (this.higherSlider != null)
        {
            this.higherSlider.Loaded -= this.Slider_Loaded;
            this.higherSlider.ValueChanged -= this.HigherSlider_ValueChanged;
        }

        this.higherSlider = this.Template.FindName(PARTHigherSlider, this) as Slider;
        if (this.higherSlider != null)
        {
            this.higherSlider.Loaded += this.Slider_Loaded;
            this.higherSlider.ValueChanged += this.HigherSlider_ValueChanged;
            this.higherSlider.ApplyTemplate();
        }
    }

    protected virtual void OnHigherValueChanged(double oldValue, double newValue)
    {
        this.AdjustView();

        RoutedEventArgs args = new RoutedEventArgs();
        args.RoutedEvent = HigherValueChangedEvent;
        this.RaiseEvent(args);
    }

    protected virtual void OnLowerValueChanged(double oldValue, double newValue)
    {
        this.AdjustView();

        RoutedEventArgs args = new RoutedEventArgs();
        args.RoutedEvent = RangeSlider.LowerValueChangedEvent;
        this.RaiseEvent(args);
    }

    protected override void OnMaximumChanged(double oldValue, double newValue) => this.AdjustView();

    protected override void OnMinimumChanged(double oldValue, double newValue) =>
        // adjust the range width
        this.AdjustView();

    private static object OnCoerceLowerValueChanged(DependencyObject d, object basevalue)
    {
        var rangeSlider = (RangeSlider)d;
        if ((rangeSlider == null) || !rangeSlider.IsLoaded)
        {
            return basevalue;
        }

        var min = Math.Min(rangeSlider.Minimum, rangeSlider.Maximum);
        var max = Math.Max(rangeSlider.Minimum, rangeSlider.Maximum);
        var lowerValue = Math.Max(rangeSlider.Minimum, Math.Min(rangeSlider.Maximum, (double)basevalue));
        lowerValue = Math.Min((double)basevalue, rangeSlider.HigherValue);

        return lowerValue;
    }

    private static void OnLowerValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is RangeSlider rangeSlider)
        {
            rangeSlider.OnLowerValueChanged((double)args.OldValue, (double)args.NewValue);
        }
    }

    private static object OnCoerceHigherValueChanged(DependencyObject d, object basevalue)
    {
        var rangeSlider = (RangeSlider)d;
        if ((rangeSlider == null) || !rangeSlider.IsLoaded)
        {
            return basevalue;
        }

        return Math.Max(rangeSlider.LowerValue, (double)basevalue);
    }

    private static void OnHigherValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is RangeSlider rangeSlider)
        {
            rangeSlider.OnHigherValueChanged((double)args.OldValue, (double)args.NewValue);
        }
    }

    private void MidRange_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        this.midRangeMouseDownPoint = e.GetPosition(this.midRange);
        this.midRangeMouseDown = true;
    }

    private void MidRange_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) => this.midRangeMouseDown = false;

    private void MidRange_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!this.midRangeMouseDown)
        {
            return;
        }

        var pos = e.GetPosition(this.midRange);

        var newMin = this.LowerValue + (((pos.X - this.midRangeMouseDownPoint.X) / this.ActualWidth) * (this.Maximum - this.Minimum));
        var newMax = this.HigherValue + (((pos.X - this.midRangeMouseDownPoint.X) / this.ActualWidth) * (this.Maximum - this.Minimum));

        if (newMin >= this.Minimum && newMax <= this.Maximum)
        {
            this.LowerValue = newMin;
            this.HigherValue = newMax;
        }
    }

    private void AdjustView()
    {
        var cv = this.GetCoercedValues();

        var thumbWidth = 10;
        double width = this.ActualWidth - thumbWidth;

        this.SetLowerSliderValues(cv.LowerValue, cv.Minimum, cv.Maximum);
        this.SetHigherSliderValues(cv.HigherValue, cv.Minimum, cv.Maximum);

        double range = cv.Maximum - cv.Minimum;

        if (range > 0)
        {
            this.RangeWidth = (width * (cv.HigherValue - cv.LowerValue)) / range;
            this.RangeMargin = new Thickness((((cv.LowerValue - this.Minimum) / range) * width) + (thumbWidth * 0.5), 0, 0, 0);
        }
        else
        {
            this.RangeWidth = 0d;
        }
    }

    private CoercedValues GetCoercedValues()
    {
        CoercedValues cv = default;
        cv.Minimum = Math.Min(this.Minimum, this.Maximum);
        cv.Maximum = Math.Max(cv.Minimum, this.Maximum);
        cv.LowerValue = Math.Max(cv.Minimum, Math.Min(cv.Maximum, this.LowerValue));
        cv.HigherValue = Math.Max(cv.Minimum, Math.Min(cv.Maximum, this.HigherValue));
        cv.HigherValue = Math.Max(cv.LowerValue, cv.HigherValue);

        return cv;
    }

    private void SetLowerSliderValues(double value, double? minimum, double? maximum)
        => this.SetSliderValues(this.lowerSlider, this.LowerSlider_ValueChanged, value, minimum, maximum);

    private void SetHigherSliderValues(double value, double? minimum, double? maximum)
        => this.SetSliderValues(this.higherSlider, this.HigherSlider_ValueChanged, value, minimum, maximum);

    private void SetSliderValues(
        Slider? slider,
        RoutedPropertyChangedEventHandler<double> handler,
        double value,
        double? minimum,
        double? maximum)
    {
        if (slider != null)
        {
            slider.ValueChanged -= handler;

            slider.Value = value;
            if (minimum != null)
            {
                slider.Minimum = minimum.Value;
            }

            if (maximum != null)
            {
                slider.Maximum = maximum.Value;
            }

            slider.ValueChanged += handler;
        }
    }

    private void UpdateHigherValue(double? value)
    {
        CoercedValues cv = this.GetCoercedValues();
        double newValue = Math.Max(cv.Minimum, Math.Min(cv.Maximum, value.HasValue ? value.Value : 0d));
        newValue = Math.Max(newValue, cv.LowerValue);
        this.SetHigherSliderValues(newValue, null, null);
        this.HigherValue = newValue;
    }

    private void UpdateLowerValue(double? value)
    {
        CoercedValues cv = this.GetCoercedValues();
        double newValue = Math.Max(cv.Minimum, Math.Min(cv.Maximum, value.HasValue ? value.Value : 0d));
        newValue = Math.Min(newValue, cv.HigherValue);
        this.SetLowerSliderValues(newValue, null, null);
        this.LowerValue = newValue;
    }

    private void RangeSlider_SizeChanged(object sender, SizeChangedEventArgs e) => this.AdjustView();

    private void Slider_Loaded(object sender, RoutedEventArgs e) => this.AdjustView();

    private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if ((this.lowerSlider != null) && this.lowerSlider.IsLoaded)
        {
            this.UpdateLowerValue(e.NewValue);
        }
    }

    private void HigherSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if ((this.higherSlider != null) && this.higherSlider.IsLoaded)
        {
            this.UpdateHigherValue(e.NewValue);
        }
    }

    private struct CoercedValues
    {
        public double Minimum;
        public double Maximum;
        public double LowerValue;
        public double HigherValue;
    }
}
