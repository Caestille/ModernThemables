using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ModernThemables.Controls
{
	[TemplatePart(Name = PART_MidRange, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_HigherSlider, Type = typeof(Slider))]
    [TemplatePart(Name = PART_LowerSlider, Type = typeof(Slider))]
    [TemplatePart(Name = PART_Track, Type = typeof(Track))]

    public class RangeSlider : Control
    {
        #region Members

        private const String PART_MidRange = "PART_MidRange";
        private const String PART_HigherSlider = "PART_HigherSlider";
        private const String PART_LowerSlider = "PART_LowerSlider";
        private const String PART_Track = "PART_Track";

        private RepeatButton _midRange;
        private Slider _lowerSlider;
        private Slider _higherSlider;
        private Track _lowerTrack;
        private Track _higherTrack;

        #endregion Members

        #region Constructors

        static RangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(typeof(RangeSlider)));
        }

        public RangeSlider()
        {
            this.SizeChanged += this.RangeSlider_SizeChanged;
        }

        #endregion Constructors

        #region Properties

        #region AutoToolTipPlacement

        public static readonly DependencyProperty AutoToolTipPlacementProperty = DependencyProperty.Register("AutoToolTipPlacement", typeof(AutoToolTipPlacement), typeof(RangeSlider),
            new FrameworkPropertyMetadata(AutoToolTipPlacement.None, RangeSlider.OnAutoToolTipPlacementChanged));

        public AutoToolTipPlacement AutoToolTipPlacement
        {
            get
            {
                return (AutoToolTipPlacement)GetValue(RangeSlider.AutoToolTipPlacementProperty);
            }
            set
            {
                SetValue(RangeSlider.AutoToolTipPlacementProperty, value);
            }
        }

        private static void OnAutoToolTipPlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var rangeSlider = sender as RangeSlider;
            if (rangeSlider != null)
            {
                rangeSlider.OnAutoToolTipPlacementChanged((AutoToolTipPlacement)e.OldValue, (AutoToolTipPlacement)e.NewValue);
            }
        }

        protected virtual void OnAutoToolTipPlacementChanged(AutoToolTipPlacement oldValue, AutoToolTipPlacement newValue)
        {
        }

        #endregion //AutoToolTipPlacement

        #region AutoToolTipPrecision

        public static readonly DependencyProperty AutoToolTipPrecisionProperty = DependencyProperty.Register("AutoToolTipPrecision", typeof(int), typeof(RangeSlider),
            new FrameworkPropertyMetadata(0));

        public int AutoToolTipPrecision
        {
            get
            {
                return (int)GetValue(RangeSlider.AutoToolTipPrecisionProperty);
            }
            set
            {
                SetValue(RangeSlider.AutoToolTipPrecisionProperty, value);
            }
        }

        #endregion //AutoToolTipPrecision

        #region HigherValue
        /// <summary>
        /// HigherValue property represents the higher value within the selected range.
        /// </summary>
        public static readonly DependencyProperty HigherValueProperty = DependencyProperty.Register("HigherValue", typeof(double), typeof(RangeSlider)
          , new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, RangeSlider.OnHigherValueChanged, RangeSlider.OnCoerceHigherValueChanged));

        public double HigherValue
        {
            get
            {
                return (double)GetValue(RangeSlider.HigherValueProperty);
            }
            set
            {
                SetValue(RangeSlider.HigherValueProperty, value);
            }
        }

        private static object OnCoerceHigherValueChanged(DependencyObject d, object basevalue)
        {
            var rangeSlider = (RangeSlider)d;
            if ((rangeSlider == null) || !rangeSlider.IsLoaded)
                return basevalue;

            var min = Math.Min(rangeSlider.Minimum, rangeSlider.Maximum);
            var max = Math.Max(rangeSlider.Minimum, rangeSlider.Maximum);
            var higherValue = Math.Max(rangeSlider.Minimum, Math.Min(rangeSlider.Maximum, (double)basevalue));
            higherValue = Math.Max(rangeSlider.LowerValue, (double)basevalue);

            return higherValue;
        }

        private static void OnHigherValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RangeSlider rangeSlider = sender as RangeSlider;
            if (rangeSlider != null)
            {
                rangeSlider.OnHigherValueChanged((double)args.OldValue, (double)args.NewValue);
            }
        }

        protected virtual void OnHigherValueChanged(double oldValue, double newValue)
        {
            this.AdjustView();

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = RangeSlider.HigherValueChangedEvent;
            this.RaiseEvent(args);
        }

        #endregion HigherValue

        #region LowerValue
        /// <summary>
        /// LowerValue property represents the lower value within the selected range.
        /// </summary>
        public static readonly DependencyProperty LowerValueProperty = DependencyProperty.Register("LowerValue", typeof(double), typeof(RangeSlider)
          , new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, RangeSlider.OnLowerValueChanged, RangeSlider.OnCoerceLowerValueChanged));

        public double LowerValue
        {
            get
            {
                return (double)GetValue(RangeSlider.LowerValueProperty);
            }
            set
            {
                SetValue(RangeSlider.LowerValueProperty, value);
            }
        }

        private static object OnCoerceLowerValueChanged(DependencyObject d, object basevalue)
        {
            var rangeSlider = (RangeSlider)d;
            if ((rangeSlider == null) || !rangeSlider.IsLoaded)
                return basevalue;

            var min = Math.Min(rangeSlider.Minimum, rangeSlider.Maximum);
            var max = Math.Max(rangeSlider.Minimum, rangeSlider.Maximum);
            var lowerValue = Math.Max(rangeSlider.Minimum, Math.Min(rangeSlider.Maximum, (double)basevalue));
            lowerValue = Math.Min((double)basevalue, rangeSlider.HigherValue);

            return lowerValue;
        }

        private static void OnLowerValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RangeSlider rangeSlider = sender as RangeSlider;
            if (rangeSlider != null)
            {
                rangeSlider.OnLowerValueChanged((double)args.OldValue, (double)args.NewValue);
            }
        }

        protected virtual void OnLowerValueChanged(double oldValue, double newValue)
        {
            this.AdjustView();

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = RangeSlider.LowerValueChangedEvent;
            this.RaiseEvent(args);
        }

        #endregion LowerValue

        #region Maximum
        /// <summary>
        /// Maximum property represents the maximum value, which can be selected, in a range.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSlider)
          , new FrameworkPropertyMetadata(RangeSlider.OnMaximumChanged));

        public double Maximum
        {
            get
            {
                return (double)GetValue(RangeSlider.MaximumProperty);
            }
            set
            {
                SetValue(RangeSlider.MaximumProperty, value);
            }
        }

        private static void OnMaximumChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RangeSlider rangeSlider = sender as RangeSlider;
            if (rangeSlider != null)
            {
                rangeSlider.OnMaximumChanged((double)args.OldValue, (double)args.NewValue);
            }
        }

        protected virtual void OnMaximumChanged(double oldValue, double newValue)
        {
            this.AdjustView();
        }

        #endregion Maximum

        #region Minimum
        /// <summary>
        /// Minimum property represents the minimum value, which can be selected, in a range.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSlider)
          , new FrameworkPropertyMetadata(RangeSlider.OnMinimumChanged));

        public double Minimum
        {
            get
            {
                return (double)GetValue(RangeSlider.MinimumProperty);
            }
            set
            {
                SetValue(RangeSlider.MinimumProperty, value);
            }
        }

        private static void OnMinimumChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RangeSlider rangeSlider = sender as RangeSlider;
            if (rangeSlider != null)
            {
                rangeSlider.OnMinimumChanged((double)args.OldValue, (double)args.NewValue);
            }
        }

        protected virtual void OnMinimumChanged(double oldValue, double newValue)
        {
            // adjust the range width
            this.AdjustView();
        }

        #endregion Minimum

        #region RangeWidth
        /// <summary>
        /// RangeWidth property is a readonly property, used to calculate the percentage of the range within the entire min/max range.
        /// </summary>

        private static readonly DependencyPropertyKey RangeWidthPropertyKey = DependencyProperty.RegisterAttachedReadOnly("RangeWidth", typeof(double)
          , typeof(RangeSlider), new PropertyMetadata(0d));

        public static readonly DependencyProperty RangeWidthProperty = RangeWidthPropertyKey.DependencyProperty;

        public double RangeWidth
        {
            get
            {
                return (double)GetValue(RangeSlider.RangeWidthProperty);
            }
            private set
            {
                SetValue(RangeSlider.RangeWidthPropertyKey, value);
            }
        }

        #endregion RangeWidth

        #region RangeMargin
        /// <summary>
        /// RangeMargin property is a readonly property, used to calculate the offset of the range within the left hand side of the range.
        /// </summary>

        private static readonly DependencyPropertyKey RangeMarginPropertyKey = DependencyProperty.RegisterAttachedReadOnly("RangeMargin", typeof(Thickness)
          , typeof(RangeSlider), new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty RangeMarginProperty = RangeMarginPropertyKey.DependencyProperty;

        public Thickness RangeMargin
        {
            get
            {
                return (Thickness)GetValue(RangeSlider.RangeMarginProperty);
            }
            private set
            {
                SetValue(RangeSlider.RangeMarginPropertyKey, value);
            }
        }

        #endregion RangeWidth

        #endregion Properties

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_midRange != null)
            {
                //_midRange.Click -= new RoutedEventHandler(this.HigherRange_Click);
            }
            _midRange = this.Template.FindName(PART_MidRange, this) as RepeatButton;
            if (_midRange != null)
            {
                //_midRange.Click += new RoutedEventHandler(this.HigherRange_Click);
            }

            if (_lowerSlider != null)
            {
                _lowerSlider.Loaded -= this.Slider_Loaded;
                _lowerSlider.ValueChanged -= LowerSlider_ValueChanged;
            }
            _lowerSlider = this.Template.FindName(PART_LowerSlider, this) as Slider;
            if (_lowerSlider != null)
            {
                _lowerSlider.Loaded += this.Slider_Loaded;
                _lowerSlider.ValueChanged += LowerSlider_ValueChanged;
                _lowerSlider.ApplyTemplate();
                _lowerTrack = _lowerSlider.Template.FindName(PART_Track, _lowerSlider) as Track;
            }

            if (_higherSlider != null)
            {
                _higherSlider.Loaded -= this.Slider_Loaded;
                _higherSlider.ValueChanged -= HigherSlider_ValueChanged;
            }
            _higherSlider = this.Template.FindName(PART_HigherSlider, this) as Slider;
            if (_higherSlider != null)
            {
                _higherSlider.Loaded += this.Slider_Loaded;
                _higherSlider.ValueChanged += HigherSlider_ValueChanged;
                _higherSlider.ApplyTemplate();
                _higherTrack = _higherSlider.Template.FindName(PART_Track, _higherSlider) as Track;
            }
        }

        #endregion Override

        #region Methods

        private void AdjustView(bool isHigherValueChanged = false)
        {
            //Coerce values to make them consistent.
            var cv = this.GetCoercedValues();

            double actualWidth = this.ActualWidth - 16;
            double lowerSliderThumbWidth = 0d;
            double higherSliderThumbWidth = 0d;

            actualWidth -= (lowerSliderThumbWidth + higherSliderThumbWidth);
            this.SetLowerSliderValues(cv.LowerValue, cv.Minimum, cv.Maximum);
            this.SetHigherSliderValues(cv.HigherValue, cv.Minimum, cv.Maximum);

            double entireRange = cv.Maximum - cv.Minimum;

            if (entireRange > 0)
            {
                var higherValue = cv.HigherValue;
                var lowerValue = cv.LowerValue;
                this.RangeWidth = (actualWidth * (higherValue - lowerValue)) / entireRange;
                this.RangeMargin = new Thickness(((lowerValue - Minimum) / entireRange) * actualWidth, 0, 0, 0);
            }
            else
            {
                this.RangeWidth = 0d;
            }
        }

        private CoercedValues GetCoercedValues()
        {
            var buffer = (Maximum - Minimum) * 0.01;
            CoercedValues cv = new CoercedValues();
            cv.Minimum = Math.Min(this.Minimum, this.Maximum);
            cv.Maximum = Math.Max(cv.Minimum, this.Maximum);
            cv.LowerValue = Math.Max(cv.Minimum, Math.Min(cv.Maximum, this.LowerValue));
            cv.HigherValue = Math.Max(cv.Minimum, Math.Min(cv.Maximum, this.HigherValue));
            cv.HigherValue = Math.Max(cv.LowerValue, cv.HigherValue);

            return cv;
        }

        private void SetLowerSliderValues(double value, double? minimum, double? maximum)
        {
            this.SetSliderValues(_lowerSlider, this.LowerSlider_ValueChanged, value, minimum, maximum);
        }

        private void SetHigherSliderValues(double value, double? minimum, double? maximum)
        {
            this.SetSliderValues(_higherSlider, this.HigherSlider_ValueChanged, value, minimum, maximum);
        }

        private void SetSliderValues(
          Slider slider,
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

        #endregion

        #region Events

        public static readonly RoutedEvent LowerValueChangedEvent = EventManager.RegisterRoutedEvent("LowerValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RangeSlider));
        public event RoutedEventHandler LowerValueChanged
        {
            add
            {
                AddHandler(RangeSlider.LowerValueChangedEvent, value);
            }
            remove
            {
                RemoveHandler(RangeSlider.LowerValueChangedEvent, value);
            }
        }

        public static readonly RoutedEvent HigherValueChangedEvent = EventManager.RegisterRoutedEvent("HigherValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RangeSlider));
        public event RoutedEventHandler HigherValueChanged
        {
            add
            {
                AddHandler(RangeSlider.HigherValueChangedEvent, value);
            }
            remove
            {
                RemoveHandler(RangeSlider.HigherValueChangedEvent, value);
            }
        }

        #endregion //Events

        #region Events Handlers

        private void RangeSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.AdjustView();
        }

        private void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            this.AdjustView();
        }

        private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((_lowerSlider != null) && _lowerSlider.IsLoaded)
            {
                this.UpdateLowerValue(e.NewValue);
            }
        }

        private void HigherSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((_higherSlider != null) && _higherSlider.IsLoaded)
            {
                this.UpdateHigherValue(e.NewValue);
            }
        }

        #endregion Events Handlers

        private struct CoercedValues
        {
            public double Minimum;
            public double Maximum;
            public double LowerValue;
            public double HigherValue;
        }
    }
}