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
using System.Diagnostics;

namespace ModernThemables.Controls
{
	[TemplatePart(Name = PART_textbox, Type = typeof(TextBox))]

	public class DatetimeTextBox : Control
	{
		#region Members

		private const string PART_textbox = "PART_textbox";

		private bool dateValid;
		private bool monthValid;
		private bool yearValid;
		private bool hourValid;
		private bool minuteValid;
		private bool secondValid;

		private TextBox textbox;

		private bool blockUpdate;

		private Dictionary<string, Brush> cachedBrushes = new();
		private Dictionary<string, Binding> cachedBindings = new();
		private Dictionary<string, bool> validCache = new();

		private KeepAliveTriggerService trigger;
		private bool blockRefresh = false;
		private DateTime lastUpdateTime = new DateTime();

		private bool isKeyboardUpdate = false;

		#endregion Members

		#region Constructors

		static DatetimeTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DatetimeTextBox), new FrameworkPropertyMetadata(typeof(DatetimeTextBox)));
		}

		public DatetimeTextBox()
		{
			trigger = new KeepAliveTriggerService(() => { blockRefresh = true; CalculateDate(false); blockRefresh = false; }, 100);
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
		}

		#endregion Constructors

		#region Properties

		public static readonly DependencyProperty DateTimeProperty = DependencyProperty.Register("DateTime", typeof(DateTime?), typeof(DatetimeTextBox),
		   new FrameworkPropertyMetadata(null, OnSetDateTime));

		public DateTime? DateTime
		{
			get => (DateTime?)GetValue(DateTimeProperty);
			set => SetValue(DateTimeProperty, value);
		}

		private static void OnSetDateTime(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var dtTb = sender as DatetimeTextBox;
			if (dtTb != null)
			{
				if (!dtTb.isKeyboardUpdate)
					Keyboard.ClearFocus();

				if ((e.OldValue == null || e.NewValue != e.OldValue)
					&& e.NewValue is DateTime dt
					&& dtTb.textbox != null && !dtTb.textbox.IsFocused)
				{
					dtTb.blockUpdate = true;
					if (!dtTb.isKeyboardUpdate)
					{
						dtTb.textbox.Focusable = false;
					}
					dtTb.textbox.Text = dt.ToString("dd/MM/yyyy HH:mm:ss");
					if (!dtTb.isKeyboardUpdate)
					{
						dtTb.textbox.Focusable = true;
						dtTb.blockUpdate = false;
					}
				}
			}
		}

		public static readonly DependencyProperty TextboxBorderThicknessProperty = DependencyProperty.Register("TextboxBorderThickness", typeof(Thickness), typeof(DatetimeTextBox),
		   new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 1)));

		public Thickness TextboxBorderThickness
		{
			get => (Thickness)GetValue(TextboxBorderThicknessProperty);
			set => SetValue(TextboxBorderThicknessProperty, value);
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
					textbox.Text = DateTime.Value.ToString("dd/MM/yyyy HH:mm:ss");
					textbox.Focusable = true;
					blockUpdate = false;
				}
			}
		}

		#endregion Override

		#region Methods

		#endregion

		#region Events

		public static readonly RoutedEvent DateChangedEvent = EventManager.RegisterRoutedEvent("DateChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DatetimeTextBox));
		public event RoutedEventHandler LowerValueChanged
		{
			add
			{
				AddHandler(DatetimeTextBox.DateChangedEvent, value);
			}
			remove
			{
				RemoveHandler(DatetimeTextBox.DateChangedEvent, value);
			}
		}

		#endregion

		#region Events Handlers

		private void TextChanged(object sender, TextChangedEventArgs e)
		{
			var skip = new List<string>() { "", " ", ":", "/" };
			var moveOnIndex = new List<int>() { 2, 5, 10, 11, 13, 16 };
			if (moveOnIndex.Contains(textbox.SelectionStart) && skip.Contains(GetNextCharacter(textbox.SelectionStart))) textbox.SelectionStart++;
			dateValid = System.DateTime.TryParseExact(textbox.Text, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
			FormatText(PART_textbox, textbox, dateValid);
			CalculateDate();
		}

		private void TextKeyDown(object sender, KeyEventArgs e)
		{
			var skip = new List<string>() { "", " ", ":", "/" };
			string text = textbox.Text == string.Empty ? "/// ::" : textbox.Text;

			if (textbox.SelectionLength == 0)
			{
				switch (e.Key)
				{
					case Key.Back:
						while (skip.Contains(GetPreviousCharacter(textbox.SelectionStart)) && textbox.SelectionStart != 0) textbox.SelectionStart--;
						e.Handled = textbox.SelectionStart == 0;
						break;
					case Key.Delete:
						while (skip.Contains(GetNextCharacter(textbox.SelectionStart)) && textbox.SelectionStart != textbox.Text.Length) textbox.SelectionStart++;
						e.Handled = textbox.SelectionStart == textbox.Text.Length;
						break;
					default:
						var key = e.Key.ToString();
						e.Handled = !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Regex.IsMatch(key, "^[A-Z]$") || e.Key == Key.Space;
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
						text = text.Replace(toDelete, updated);
						textbox.SelectionLength = 0;
						e.Handled = true;
						break;
					case Key.Delete:
						var toDelete2 = textbox.Text.Substring(textbox.SelectionStart, textbox.SelectionLength);
						var updated2 = Regex.Replace(toDelete2, "[0-9]", "");
						text = text.Replace(toDelete2, updated2);
						textbox.SelectionStart = textbox.SelectionStart + textbox.SelectionLength;
						textbox.SelectionLength = 0;
						e.Handled = true;
						break;
					default:
						var key = e.Key.ToString();
						e.Handled = !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Regex.IsMatch(key, "^[A-Z]$") || e.Key == Key.Space;
						break;
				}
			}

			textbox.Text = text;
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

			if (!blockRefresh)
			{
				trigger.Refresh();
				if (System.DateTime.Now - lastUpdateTime < TimeSpan.FromMilliseconds(50)) return;
			}

			isKeyboardUpdate = keyboardUpdate;

			var calculate = dateValid;

			Application.Current.Dispatcher.Invoke(() => { 
				if (calculate) DateTime = System.DateTime.ParseExact(textbox.Text, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
				else DateTime = null;
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