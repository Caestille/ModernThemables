using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.Generic;
using System.Windows.Input;
using CoreUtilities.Services;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;

namespace ModernThemables.Controls
{
	[TemplatePart(Name = PART_textbox, Type = typeof(TextBox))]

	public class DatetimeTextBox : Control
	{
		#region Members

		private const string PART_textbox = "PART_textbox";

		private bool dateValid;

		private TextBox textbox;

		private bool blockUpdate;

		private Dictionary<string, Brush> cachedBrushes = new();
		private Dictionary<string, Binding> cachedBindings = new();
		private Dictionary<string, bool> validCache = new();

		private KeepAliveTriggerService trigger;

		private bool isKeyboardUpdate = false;

		private bool blockRecalculateOnce;

		private List<string> skipCharacters = new() { "", " ", ":", "/" };

		private DateTime? lastValue;

		#endregion Members

		#region Constructors

		static DatetimeTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DatetimeTextBox), new FrameworkPropertyMetadata(typeof(DatetimeTextBox)));
		}

		public DatetimeTextBox()
		{
			trigger = new KeepAliveTriggerService(() => { CalculateDate(false); }, 100);
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
			this.GotKeyboardFocus += DatetimeTextBox_GotKeyboardFocus;
		}

		private void DatetimeTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (textbox!= null && !textbox.IsKeyboardFocused)
			{
				textbox.Focus();
			}
		}

		#endregion Constructors

		#region Properties

		public DateTime? DateTime
		{
			get => (DateTime?)GetValue(DateTimeProperty);
			set => SetValue(DateTimeProperty, value);
		}

		public static readonly DependencyProperty DateTimeProperty = DependencyProperty.Register(
			"DateTime",
			typeof(DateTime?),
			typeof(DatetimeTextBox),
			new FrameworkPropertyMetadata(null, OnSetDateTime));

		public string Format
		{
			get => (string)GetValue(FormatProperty);
			set => SetValue(FormatProperty, value);
		}

		public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
			"Format",
			typeof(string),
			typeof(DatetimeTextBox),
			new FrameworkPropertyMetadata("dd/MM/yyyy HH:mm:ss", OnSetFormat));

		private static void OnSetFormat(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var _this = sender as DatetimeTextBox;
			if (_this != null)
			{
				_this.blockRecalculateOnce = true;
				_this.textbox.Text = _this.DateTime.HasValue ? _this.DateTime.Value.ToString(_this.Format) : "";
			}
		}

		private static void OnSetDateTime(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var _this = sender as DatetimeTextBox;
			if (_this != null)
			{
				if (!_this.isKeyboardUpdate && _this.IsKeyboardFocused)
					Keyboard.ClearFocus();

				if ((e.OldValue == null || e.NewValue != e.OldValue)
					&& e.NewValue is DateTime dt
					&& _this.textbox != null && !_this.textbox.IsFocused)
				{
					_this.blockUpdate = true;
					if (!_this.isKeyboardUpdate)
					{
						_this.textbox.Focusable = false;
					}
					_this.textbox.Text = dt.ToString(_this.Format);
					if (!_this.isKeyboardUpdate)
					{
						_this.textbox.Focusable = true;
					}
					_this.blockUpdate = false;
				}
			}
		}

		#endregion Properties

		#region Override

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (textbox != null)
			{
				textbox.TextChanged -= TextChanged;
				textbox.PreviewKeyDown -= TextKeyDown;
			}
			textbox = this.Template.FindName(PART_textbox, this) as TextBox;
			if (textbox != null)
			{
				textbox.TextChanged += TextChanged;
				textbox.PreviewKeyDown += TextKeyDown;
			}

			if (textbox != null)
			{
				if (DateTime != null)
				{
					blockUpdate = true;
					textbox.Focusable = false;
					textbox.Text = DateTime.Value.ToString(Format);
					textbox.Focusable = true;
					blockUpdate = false;
				}
			}
		}

		#endregion Override

		#region Register events

		public static readonly RoutedEvent DateChangedEvent = EventManager.RegisterRoutedEvent(
			"DateChanged",
			RoutingStrategy.Bubble,
			typeof(RoutedPropertyChangedEventHandler<DateTime?>),
			typeof(DatetimeTextBox));

		public event RoutedPropertyChangedEventHandler<DateTime?> DateChanged
		{
			add => AddHandler(DatetimeTextBox.DateChangedEvent, value);
			remove =>RemoveHandler(DatetimeTextBox.DateChangedEvent, value);
		}

		#endregion

		#region Events Handlers

		private void TextChanged(object sender, TextChangedEventArgs e)
		{
			if (blockRecalculateOnce)
			{
				blockRecalculateOnce = false;
				return;
			}

			if (textbox.Text.ToCharArray().Select(x => x.ToString()).All(skipCharacters.Contains))
			{
				textbox.SelectionStart = 0;
			}

			var moveOnIndex = new List<int>();
			for (int i = 0; i < Format.Length; i++)
			{
				if (skipCharacters.Contains(Format.ElementAt(i).ToString()))
				{
					moveOnIndex.Add(i);
				}
			}

			if (moveOnIndex.Contains(textbox.SelectionStart)
				&& skipCharacters.Contains(GetNextCharacter(textbox.SelectionStart)))
			{
				textbox.SelectionStart++;
			}

			dateValid = System.DateTime.TryParseExact(
				textbox.Text, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
			FormatText(PART_textbox, textbox, dateValid);
			CalculateDate();
		}

		private void TextKeyDown(object sender, KeyEventArgs e)
		{
			string text = textbox.Text == string.Empty ? "/// ::" : textbox.Text;
			int selectStart = textbox.SelectionStart;
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

			if (textbox.SelectionLength == 0)
			{
				switch (e.Key)
				{
					case Key.Back:
						while (skipCharacters.Contains(GetPreviousCharacter(textbox.SelectionStart)) && textbox.SelectionStart != 0) textbox.SelectionStart--;
						e.Handled = textbox.SelectionStart == 0;
						break;
					case Key.Delete:
						while (skipCharacters.Contains(GetNextCharacter(textbox.SelectionStart)) && textbox.SelectionStart != textbox.Text.Length) textbox.SelectionStart++;
						e.Handled = textbox.SelectionStart == textbox.Text.Length;
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
						var toDelete = textbox.Text.Substring(textbox.SelectionStart, textbox.SelectionLength);
						var updated = Regex.Replace(toDelete, "[0-9]", "");
						text = $"{text.Substring(0, textbox.SelectionStart)}{updated}{text.Substring(textbox.SelectionStart + textbox.SelectionLength, text.Length - textbox.SelectionStart - textbox.SelectionLength)}";
						selectStart = textbox.SelectionStart;
						setStart = true;
						textbox.SelectionLength = 0;
						e.Handled = true;
						break;
					case Key.Delete:
						var toDelete2 = textbox.Text.Substring(textbox.SelectionStart, textbox.SelectionLength);
						var updated2 = Regex.Replace(toDelete2, "[0-9]", "");
						text = $"{text.Substring(0, textbox.SelectionStart)}{updated2}{text.Substring(textbox.SelectionStart + textbox.SelectionLength, text.Length - textbox.SelectionStart - textbox.SelectionLength)}";
						selectStart = textbox.SelectionStart + textbox.SelectionLength;
						textbox.SelectionLength = 0;
						setStart = true;
						e.Handled = true;
						break;
					default:
						if (Regex.IsMatch(key, "D[0-9]"))
						{
							var toDelete3 = textbox.Text.Substring(textbox.SelectionStart, textbox.SelectionLength);
							var updated3 = Regex.Replace(toDelete3, "[0-9a-zA-Z]+", "");
							text = $"{text.Substring(0, textbox.SelectionStart)}{updated3}{text.Substring(textbox.SelectionStart + textbox.SelectionLength, text.Length - textbox.SelectionStart - textbox.SelectionLength)}";
							selectStart = textbox.SelectionStart;
							setStart = true;
							e.Handled = !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Regex.IsMatch(key, "^[A-Z]$") || e.Key == Key.Space;
						}
						break;
				}
			}

			textbox.Text = text;
			if (setStart) textbox.SelectionStart = selectStart;

		}

		private string GetPreviousCharacter(int currentPos)
		{
			var prev = textbox.Text.ToCharArray()[Math.Max(0, currentPos - 1)].ToString();
			return prev;
		}

		private string GetNextCharacter(int currentPos)
		{
			var next = textbox.Text.ToCharArray()[Math.Min(textbox.Text.Length - 1, currentPos)].ToString();
			return next;
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			trigger.Stop();
		}

		#endregion Events Handlers

		private void CalculateDate(bool keyboardUpdate = true)
		{
			if (blockUpdate) return;

			isKeyboardUpdate = keyboardUpdate;

			Application.Current.Dispatcher.Invoke(() => {
				DateTime? newVal = null;
				if (dateValid) newVal = System.DateTime.ParseExact(textbox.Text, Format, CultureInfo.InvariantCulture);

				if (newVal != DateTime)
				{
					if (DateTime != null)
					{
						lastValue = DateTime.Value;
					}
					DateTime = newVal;
					var args = new RoutedPropertyChangedEventArgs<DateTime?>(lastValue, newVal, DateChangedEvent) { Source = this };
                    this.RaiseEvent(args);
				}
            });

			isKeyboardUpdate = false;
		}

		private void FormatText(string textBoxName, TextBox textBox, bool valid)
		{
			if (!valid && (!validCache.ContainsKey(textBoxName) || validCache[textBoxName] != valid))
			{
				BindingExpression expression = textBox.GetBindingExpression(TextBox.ForegroundProperty);
				if (expression != null)
				{
					Binding binding = expression.ParentBinding;
					cachedBindings[textBoxName] = binding;
				}
				cachedBrushes[textBoxName] = textBox.Foreground;
				textBox.Foreground = new SolidColorBrush(Colors.Red);
				validCache[textBoxName] = valid;
			}
			else if (valid && (!validCache.ContainsKey(textBoxName) || validCache[textBoxName] != valid))
			{
				if (cachedBrushes.ContainsKey(textBoxName))
				{
					textBox.Foreground = cachedBrushes[textBoxName];
				}

				if (cachedBindings.ContainsKey(textBoxName))
				{
					textBox.SetBinding(TextBox.ForegroundProperty, cachedBindings[textBoxName]);
				}
				validCache[textBoxName] = valid;
			}
		}
	}
}