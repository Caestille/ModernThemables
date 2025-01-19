namespace ModernThemables.Controls;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CoreUtilities.Services;

public class DateTimeTextBox : TextBox
{
    private bool blockUpdate;
    private readonly RefreshTrigger trigger;
    private bool isKeyboardUpdate = false;
    private bool blockRecalculateOnce;

    private readonly List<string> skipCharacters = new() { string.Empty, " ", ":", "/" };

    private DateTime? lastValue;

    public DateTimeTextBox()
    {
        this.trigger = new RefreshTrigger(() => { this.CalculateDate(false); }, 100);
        Application.Current.Dispatcher.ShutdownStarted += this.Dispatcher_ShutdownStarted;
        this.DataContextChanged += this.DatetimeTextBox_DataContextChanged;
        OnSetDateTime(this, new DependencyPropertyChangedEventArgs(DateTimeProperty, System.DateTime.MinValue, this.DateTime));
    }

    private void DatetimeTextBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (this.DataContext is null)
        {
            this.DateTime = null;
        }
    }

    public DateTime? DateTime
    {
        get => (DateTime?)this.GetValue(DateTimeProperty);
        set => this.SetValue(DateTimeProperty, value);
    }

    public static readonly DependencyProperty DateTimeProperty = DependencyProperty.Register(
        nameof(DateTime),
        typeof(DateTime?),
        typeof(DateTimeTextBox),
        new FrameworkPropertyMetadata(System.DateTime.Now, OnSetDateTime));

    public bool DateTimeValid
    {
        get => (bool)this.GetValue(DateTimeValidProperty);
        set => this.SetValue(DateTimeValidProperty, value);
    }

    public static readonly DependencyProperty DateTimeValidProperty = DependencyProperty.Register(
        nameof(DateTimeValid),
        typeof(bool),
        typeof(DateTimeTextBox),
        new FrameworkPropertyMetadata(true));

    public string Format
    {
        get => (string)this.GetValue(FormatProperty);
        set => this.SetValue(FormatProperty, value);
    }

    public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
        nameof(Format),
        typeof(string),
        typeof(DateTimeTextBox),
        new FrameworkPropertyMetadata(OnSetFormat));

    public Brush WarningBrush
    {
        get => (Brush)this.GetValue(WarningBrushProperty);
        set => this.SetValue(WarningBrushProperty, value);
    }

    public static readonly DependencyProperty WarningBrushProperty = DependencyProperty.Register(
        nameof(WarningBrush),
        typeof(Brush),
        typeof(DateTimeTextBox),
        new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red)));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)this.GetValue(CornerRadiusProperty);
        set => this.SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(DateTimeTextBox),
        new PropertyMetadata(new CornerRadius(0)));

    private static void OnSetFormat(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var _this = sender as DateTimeTextBox;
        if (_this != null)
        {
            _this.blockRecalculateOnce = true;
            _this.Text = _this.DateTime.HasValue ? _this.DateTime.Value.ToString(_this.Format) : string.Empty;
        }
    }

    private static void OnSetDateTime(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var _this = sender as DateTimeTextBox;
        if (_this != null)
        {
            if (!_this.isKeyboardUpdate && _this.IsKeyboardFocused)
            {
                Keyboard.ClearFocus();
            }

            if ((e.OldValue == null || e.NewValue != e.OldValue)
                && e.NewValue is DateTime dt
                && _this != null && !_this.IsFocused)
            {
                _this.blockUpdate = true;
                if (!_this.isKeyboardUpdate)
                {
                    _this.Focusable = false;
                }

                _this.Text = dt.ToString(_this.Format);
                if (!_this.isKeyboardUpdate)
                {
                    _this.Focusable = true;
                }

                _this.blockUpdate = false;
            }
            else if (e.NewValue == null && _this != null && !_this.isKeyboardUpdate)
            {
                _this.Text = string.Join(string.Empty, _this.Format.ToCharArray().Where(x => x == ':' || x == ' ' || x == '/'));
            }
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this.TextChanged -= this.ThisTextChanged;
        this.PreviewKeyDown -= this.TextKeyDown;
        this.TextChanged += this.ThisTextChanged;
        this.PreviewKeyDown += this.TextKeyDown;
        this.Text = this.DateTime.HasValue
            ? this.DateTime.Value.ToString(this.Format)
            : string.Join(string.Empty, this.Format.ToCharArray().Where(x => x == ':' || x == ' ' || x == '/'));

        if (this.DateTime != null)
        {
            this.blockUpdate = true;
            this.Focusable = false;
            this.Text = this.DateTime.Value.ToString(this.Format);
            this.Focusable = true;
            this.blockUpdate = false;
        }
    }

    public static readonly RoutedEvent DateChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(DateChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<DateTime?>),
        typeof(DateTimeTextBox));

    public event RoutedPropertyChangedEventHandler<DateTime?> DateChanged
    {
        add => this.AddHandler(DateChangedEvent, value);
        remove => this.RemoveHandler(DateChangedEvent, value);
    }

    private void ThisTextChanged(object sender, TextChangedEventArgs e)
    {
        if (this.blockRecalculateOnce)
        {
            this.blockRecalculateOnce = false;
            return;
        }

        if (this.Text.ToCharArray().Select(x => x.ToString()).All(this.skipCharacters.Contains))
        {
            this.SelectionStart = 0;
        }

        var moveOnIndex = new List<int>();
        for (int i = 0; i < this.Format.Length; i++)
        {
            if (this.skipCharacters.Contains(this.Format.ElementAt(i).ToString()))
            {
                moveOnIndex.Add(i);
            }
        }

        if (moveOnIndex.Contains(this.SelectionStart)
            && this.skipCharacters.Contains(this.GetNextCharacter(this.SelectionStart)))
        {
            this.SelectionStart++;
        }

        this.DateTimeValid = System.DateTime.TryParseExact(
            this.Text, this.Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        this.CalculateDate();
    }

    private void TextKeyDown(object sender, KeyEventArgs e)
    {
        string text = this.Text == string.Empty
            ? string.Join(string.Empty, this.Format.ToCharArray().Where(x => x == ':' || x == ' ' || x == '/'))
            : this.Text;
        int selectStart = this.SelectionStart;
        bool setStart = false;

        var key = e.Key.ToString();

        if (!(Regex.IsMatch(key, "D[0-9]")
            || e.Key == Key.Right
            || e.Key == Key.Left
            || e.Key == Key.End
            || e.Key == Key.Home
            || e.Key == Key.Delete
            || e.Key == Key.Back
            || e.Key == Key.Tab
            || Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key != Key.OemSemicolon && e.Key != Key.OemQuestion
            || Keyboard.IsKeyDown(Key.RightCtrl) && e.Key != Key.OemSemicolon && e.Key != Key.OemQuestion
            || Keyboard.IsKeyDown(Key.LeftShift) && e.Key != Key.OemSemicolon && e.Key != Key.OemQuestion
            || Keyboard.IsKeyDown(Key.RightShift) && e.Key != Key.OemSemicolon && e.Key != Key.OemQuestion))
        {
            e.Handled = true;
            return;
        }

        if (this.SelectionLength == 0)
        {
            switch (e.Key)
            {
                case Key.Back:
                    while (this.skipCharacters.Contains(this.GetPreviousCharacter(this.SelectionStart)) && this.SelectionStart != 0)
                    {
                        this.SelectionStart--;
                    }

                    e.Handled = this.SelectionStart == 0;
                    break;
                case Key.Delete:
                    while (this.skipCharacters.Contains(this.GetNextCharacter(this.SelectionStart)) && this.SelectionStart != this.Text.Length)
                    {
                        this.SelectionStart++;
                    }

                    e.Handled = this.SelectionStart == this.Text.Length;
                    break;
                default:
                    e.Handled = !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Regex.IsMatch(key, "^[A-Z]$");
                    break;
            }
        }
        else
        {
            switch (e.Key)
            {
                case Key.Back:
                    var toDelete = this.Text.Substring(this.SelectionStart, this.SelectionLength);
                    var updated = Regex.Replace(toDelete, "[0-9]", string.Empty);
                    text = $"{text.Substring(0, this.SelectionStart)}{updated}{text.Substring(this.SelectionStart + this.SelectionLength, text.Length - this.SelectionStart - this.SelectionLength)}";
                    selectStart = this.SelectionStart;
                    while (this.skipCharacters.Contains(this.GetNextCharacter(selectStart)))
                    {
                        selectStart++;
                    }

                    setStart = true;
                    this.SelectionLength = 0;
                    e.Handled = true;
                    break;
                case Key.Delete:
                    var toDelete2 = this.Text.Substring(this.SelectionStart, this.SelectionLength);
                    var updated2 = Regex.Replace(toDelete2, "[0-9]", string.Empty);
                    text = $"{text.Substring(0, this.SelectionStart)}{updated2}{text.Substring(this.SelectionStart + this.SelectionLength, text.Length - this.SelectionStart - this.SelectionLength)}";
                    selectStart = this.SelectionStart + this.SelectionLength;
                    this.SelectionLength = 0;
                    setStart = true;
                    e.Handled = true;
                    break;
                default:
                    if (Regex.IsMatch(key, "D[0-9]"))
                    {
                        var toDelete3 = this.Text.Substring(this.SelectionStart, this.SelectionLength);
                        var updated3 = Regex.Replace(toDelete3, "[0-9a-zA-Z]+", string.Empty);
                        text = $"{text.Substring(0, this.SelectionStart)}{updated3}{text.Substring(this.SelectionStart + this.SelectionLength, text.Length - this.SelectionStart - this.SelectionLength)}";
                        selectStart = this.SelectionStart;
                        setStart = true;
                        e.Handled = !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Regex.IsMatch(key, "^[A-Z]$") || e.Key == Key.Space;
                    }

                    break;
            }
        }

        this.Text = text;
        if (setStart)
        {
            this.SelectionStart = selectStart;
        }
    }

    private string GetPreviousCharacter(int currentPos)
    {
        var prev = this.Text.ToCharArray()[Math.Max(0, currentPos - 1)].ToString();
        return prev;
    }

    private string GetNextCharacter(int currentPos)
    {
        var next = this.Text.ToCharArray()[Math.Min(this.Text.Length - 1, currentPos)].ToString();
        return next;
    }

    private void Dispatcher_ShutdownStarted(object? sender, EventArgs e) => this.trigger.Stop();

    private void CalculateDate(bool keyboardUpdate = true)
    {
        if (this.blockUpdate)
        {
            return;
        }

        this.isKeyboardUpdate = keyboardUpdate;

        Application.Current.Dispatcher.Invoke(() =>
        {
            DateTime? newVal = null;
            if (this.DateTimeValid)
            {
                newVal = System.DateTime.ParseExact(this.Text, this.Format, CultureInfo.InvariantCulture);
            }

            if (newVal != this.DateTime)
            {
                if (this.DateTime != null)
                {
                    this.lastValue = this.DateTime.Value;
                }

                this.DateTime = newVal;
                var args = new RoutedPropertyChangedEventArgs<DateTime?>(this.lastValue, newVal, DateChangedEvent) { Source = this };
                this.RaiseEvent(args);
            }
        });

        this.isKeyboardUpdate = false;
    }
}
