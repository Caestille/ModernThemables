using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Input;
using CoreUtilities.Services;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;

namespace ModernThemables.Controls
{
	public class DateTimeTextBox : TextBox
	{
		private bool blockUpdate;
		private readonly RefreshTrigger trigger;
		private bool isKeyboardUpdate = false;
		private bool blockRecalculateOnce;

		private readonly List<string> skipCharacters = new() { "", " ", ":", "/" };

		private DateTime? lastValue;

		static DateTimeTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeTextBox), new FrameworkPropertyMetadata(typeof(DateTimeTextBox)));
		}

		public DateTimeTextBox()
		{
			trigger = new RefreshTrigger(() => { CalculateDate(false); }, 100);
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
			DataContextChanged += DatetimeTextBox_DataContextChanged;
            OnSetDateTime(this, new DependencyPropertyChangedEventArgs(DateTimeProperty, System.DateTime.MinValue, DateTime));
		}

		private void DatetimeTextBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (DataContext is null) DateTime = null;
		}

		public DateTime? DateTime
		{
			get => (DateTime?)GetValue(DateTimeProperty);
			set => SetValue(DateTimeProperty, value);
		}

		public static readonly DependencyProperty DateTimeProperty = DependencyProperty.Register(
			nameof(DateTime),
			typeof(DateTime?),
			typeof(DateTimeTextBox),
			new FrameworkPropertyMetadata(System.DateTime.Now, OnSetDateTime));

        public bool DateTimeValid
        {
            get => (bool)GetValue(DateTimeValidProperty);
            set => SetValue(DateTimeValidProperty, value);
        }

        public static readonly DependencyProperty DateTimeValidProperty = DependencyProperty.Register(
            nameof(DateTimeValid),
            typeof(bool),
            typeof(DateTimeTextBox),
            new FrameworkPropertyMetadata(true));

        public string Format
		{
			get => (string)GetValue(FormatProperty);
			set => SetValue(FormatProperty, value);
		}

		public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
			nameof(Format),
			typeof(string),
			typeof(DateTimeTextBox),
			new FrameworkPropertyMetadata(OnSetFormat));

		public Brush WarningBrush
		{
			get => (Brush)GetValue(WarningBrushProperty);
			set => SetValue(WarningBrushProperty, value);
		}

		public static readonly DependencyProperty WarningBrushProperty = DependencyProperty.Register(
			nameof(WarningBrush),
			typeof(Brush),
			typeof(DateTimeTextBox),
			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red)));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
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
				_this.Text = _this.DateTime.HasValue ? _this.DateTime.Value.ToString(_this.Format) : "";
			}
		}

		private static void OnSetDateTime(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var _this = sender as DateTimeTextBox;
			if (_this != null)
			{
				if (!_this.isKeyboardUpdate && _this.IsKeyboardFocused)
					Keyboard.ClearFocus();

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
					_this.Text = string.Join("", _this.Format.ToCharArray().Where(x => x == ':' || x == ' ' || x == '/'));
				}
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			TextChanged -= ThisTextChanged;
			PreviewKeyDown -= TextKeyDown;
			TextChanged += ThisTextChanged;
			PreviewKeyDown += TextKeyDown;
			Text = DateTime.HasValue 
				? DateTime.Value.ToString(Format)
				: string.Join("", Format.ToCharArray().Where(x => x == ':' || x == ' ' || x == '/'));

			if (DateTime != null)
			{
				blockUpdate = true;
				Focusable = false;
				Text = DateTime.Value.ToString(Format);
				Focusable = true;
				blockUpdate = false;
			}
		}

		public static readonly RoutedEvent DateChangedEvent = EventManager.RegisterRoutedEvent(
			nameof(DateChanged),
			RoutingStrategy.Bubble,
			typeof(RoutedPropertyChangedEventHandler<DateTime?>),
			typeof(DateTimeTextBox));

		public event RoutedPropertyChangedEventHandler<DateTime?> DateChanged
		{
			add => AddHandler(DateChangedEvent, value);
			remove =>RemoveHandler(DateChangedEvent, value);
		}

		private void ThisTextChanged(object sender, TextChangedEventArgs e)
		{
			if (blockRecalculateOnce)
			{
				blockRecalculateOnce = false;
				return;
			}

			if (Text.ToCharArray().Select(x => x.ToString()).All(skipCharacters.Contains))
			{
				SelectionStart = 0;
			}

			var moveOnIndex = new List<int>();
			for (int i = 0; i < Format.Length; i++)
			{
				if (skipCharacters.Contains(Format.ElementAt(i).ToString()))
				{
					moveOnIndex.Add(i);
				}
			}

			if (moveOnIndex.Contains(SelectionStart)
				&& skipCharacters.Contains(GetNextCharacter(SelectionStart)))
			{
				SelectionStart++;
			}

			DateTimeValid = System.DateTime.TryParseExact(
				Text, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
			CalculateDate();
		}

		private void TextKeyDown(object sender, KeyEventArgs e)
		{
			string text = Text == string.Empty
				? string.Join("", Format.ToCharArray().Where(x => x == ':' || x == ' ' || x == '/'))
				: Text;
			int selectStart = SelectionStart;
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

			if (SelectionLength == 0)
			{
				switch (e.Key)
				{
					case Key.Back:
						while (skipCharacters.Contains(GetPreviousCharacter(SelectionStart)) && SelectionStart != 0) SelectionStart--;
						e.Handled = SelectionStart == 0;
						break;
					case Key.Delete:
						while (skipCharacters.Contains(GetNextCharacter(SelectionStart)) && SelectionStart != Text.Length) SelectionStart++;
						e.Handled = SelectionStart == Text.Length;
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
						var toDelete = Text.Substring(SelectionStart, SelectionLength);
						var updated = Regex.Replace(toDelete, "[0-9]", "");
						text = $"{text.Substring(0, SelectionStart)}{updated}{text.Substring(SelectionStart + SelectionLength, text.Length - SelectionStart - SelectionLength)}";
						selectStart = SelectionStart;
						while (skipCharacters.Contains(GetNextCharacter(selectStart)))
						{
							selectStart++;
						}
						setStart = true;
						SelectionLength = 0;
						e.Handled = true;
						break;
					case Key.Delete:
						var toDelete2 = Text.Substring(SelectionStart, SelectionLength);
						var updated2 = Regex.Replace(toDelete2, "[0-9]", "");
						text = $"{text.Substring(0, SelectionStart)}{updated2}{text.Substring(SelectionStart + SelectionLength, text.Length - SelectionStart - SelectionLength)}";
						selectStart = SelectionStart + SelectionLength;
						SelectionLength = 0;
						setStart = true;
						e.Handled = true;
						break;
					default:
						if (Regex.IsMatch(key, "D[0-9]"))
						{
							var toDelete3 = Text.Substring(SelectionStart, SelectionLength);
							var updated3 = Regex.Replace(toDelete3, "[0-9a-zA-Z]+", "");
							text = $"{text.Substring(0, SelectionStart)}{updated3}{text.Substring(SelectionStart + SelectionLength, text.Length - SelectionStart - SelectionLength)}";
							selectStart = SelectionStart;
							setStart = true;
							e.Handled = !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Regex.IsMatch(key, "^[A-Z]$") || e.Key == Key.Space;
						}
						break;
				}
			}

            Text = text;
			if (setStart) SelectionStart = selectStart;
		}

		private string GetPreviousCharacter(int currentPos)
		{
			var prev = Text.ToCharArray()[Math.Max(0, currentPos - 1)].ToString();
			return prev;
		}

		private string GetNextCharacter(int currentPos)
		{
			var next = Text.ToCharArray()[Math.Min(Text.Length - 1, currentPos)].ToString();
			return next;
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			trigger.Stop();
		}

		private void CalculateDate(bool keyboardUpdate = true)
		{
			if (blockUpdate) return;

			isKeyboardUpdate = keyboardUpdate;

			Application.Current.Dispatcher.Invoke(() => {
				DateTime? newVal = null;
				if (DateTimeValid) newVal = System.DateTime.ParseExact(Text, Format, CultureInfo.InvariantCulture);

				if (newVal != DateTime)
				{
					if (DateTime != null)
					{
						lastValue = DateTime.Value;
					}
					DateTime = newVal;
					var args = new RoutedPropertyChangedEventArgs<DateTime?>(lastValue, newVal, DateChangedEvent) { Source = this };
					RaiseEvent(args);
				}
			});

			isKeyboardUpdate = false;
		}
	}
}