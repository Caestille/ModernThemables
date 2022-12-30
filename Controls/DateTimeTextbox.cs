﻿using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Xml.Serialization;
using System.Windows.Data;
using System.Collections.Generic;

namespace ModernThemables.Controls
{
	[TemplatePart(Name = PART_dayTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_monthTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_yearTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_hourTextBox, Type = typeof(TextBox))]
	[TemplatePart(Name = PART_minuteTextBox, Type = typeof(TextBox))]
	[TemplatePart(Name = PART_secondTextBox, Type = typeof(TextBox))]

	public class DatetimeTextBox : Control
    {
        #region Members

        private const string PART_dayTextBox = "PART_dayTextBox";
        private const string PART_monthTextBox = "PART_monthTextBox";
        private const string PART_yearTextBox = "PART_yearTextBox";
        private const string PART_hourTextBox = "PART_hourTextBox";
        private const string PART_minuteTextBox = "PART_minuteTextBox";
		private const string PART_secondTextBox = "PART_secondTextBox";

		private bool dayValid;
		private bool monthValid;
		private bool yearValid;
		private bool hourValid;
		private bool minuteValid;
		private bool secondValid;

		private TextBox dayTextBox;
        private TextBox monthTextBox;
        private TextBox yearTextBox;
        private TextBox hourTextBox;
        private TextBox minuteTextBox;
		private TextBox secondTextBox;

		private Dictionary<string, Brush> cachedBrushes = new();
		private Dictionary<string, Binding> cachedBindings = new();
		private Dictionary<string, bool> validCache = new();

		#endregion Members

		#region Constructors

		static DatetimeTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatetimeTextBox), new FrameworkPropertyMetadata(typeof(DatetimeTextBox)));
        }

        public DatetimeTextBox()
        {

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
				if (e.NewValue != e.OldValue 
					&& e.NewValue is DateTime dt
					&& ! dtTb.dayTextBox.IsFocused
					&& !dtTb.monthTextBox.IsFocused
					&& !dtTb.yearTextBox.IsFocused
					&& !dtTb.hourTextBox.IsFocused
					&& !dtTb.minuteTextBox.IsFocused
					&& !dtTb.secondTextBox.IsFocused)
				{
					dtTb.dayTextBox.Text = dt.Day.ToString().PadLeft(2, '0');
					dtTb.monthTextBox.Text = dt.Month.ToString().PadLeft(2, '0');
					dtTb.yearTextBox.Text = dt.Year.ToString().PadLeft(4, '0');
					dtTb.hourTextBox.Text = dt.Hour.ToString().PadLeft(2, '0');
					dtTb.minuteTextBox.Text = dt.Minute.ToString().PadLeft(2, '0');
					dtTb.secondTextBox.Text = dt.Second.ToString().PadLeft(2, '0');
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

            if (dayTextBox != null)
            {
				dayTextBox.TextChanged -= DayChanged;
				dayTextBox.PreviewKeyDown -= DayKeyDown;
            }
			dayTextBox = this.Template.FindName(PART_dayTextBox, this) as TextBox;
            if (dayTextBox != null)
            {
				dayTextBox.TextChanged += DayChanged;
				dayTextBox.PreviewKeyDown += DayKeyDown;
			}

			if (monthTextBox != null)
			{
				monthTextBox.TextChanged -= MonthChanged;
				monthTextBox.PreviewKeyDown -= MonthKeyDown;
			}
			monthTextBox = this.Template.FindName(PART_monthTextBox, this) as TextBox;
			if (monthTextBox != null)
			{
				monthTextBox.TextChanged += MonthChanged;
				monthTextBox.PreviewKeyDown += MonthKeyDown;
			}

			if (yearTextBox != null)
			{
				yearTextBox.TextChanged -= YearChanged;
				yearTextBox.PreviewKeyDown -= YearKeyDown;
			}
			yearTextBox = this.Template.FindName(PART_yearTextBox, this) as TextBox;
			if (yearTextBox != null)
			{
				yearTextBox.TextChanged += YearChanged;
				yearTextBox.PreviewKeyDown += YearKeyDown;
			}

			if (hourTextBox != null)
			{
				hourTextBox.TextChanged -= HourChanged;
				hourTextBox.PreviewKeyDown -= HourKeyDown;
			}
			hourTextBox = this.Template.FindName(PART_hourTextBox, this) as TextBox;
			if (hourTextBox != null)
			{
				hourTextBox.TextChanged += HourChanged;
				hourTextBox.PreviewKeyDown += HourKeyDown;
			}

			if (minuteTextBox != null)
			{
				minuteTextBox.TextChanged -= MinuteChanged;
				minuteTextBox.PreviewKeyDown -= MinuteKeyDown;
			}
			minuteTextBox = this.Template.FindName(PART_minuteTextBox, this) as TextBox;
			if (minuteTextBox != null)
			{
				minuteTextBox.TextChanged += MinuteChanged;
				minuteTextBox.PreviewKeyDown += MinuteKeyDown;
			}

			if (secondTextBox != null)
			{
				secondTextBox.TextChanged -= SecondChanged;
				secondTextBox.PreviewKeyDown -= SecondKeyDown;
			}
			secondTextBox = this.Template.FindName(PART_secondTextBox, this) as TextBox;
			if (secondTextBox != null)
			{
				secondTextBox.TextChanged += SecondChanged;
				secondTextBox.PreviewKeyDown += SecondKeyDown;
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

		private void DayChanged(object sender, TextChangedEventArgs e)
		{
			dayValid = int.TryParse(dayTextBox.Text, out int day) && day <= 31 && day > 0;
			MarkInvalid(PART_dayTextBox, dayTextBox, dayValid);
			if (dayValid && dayTextBox.Text.Length == 2)
			{
				monthTextBox.Focus();
			}
			CalculateDate();
		}

		private void DayKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Back && dayTextBox.Text.Length == 0)
			{

			}
			if (e.Key == System.Windows.Input.Key.Delete && dayTextBox.Text.Length == 0)
			{
				monthTextBox.Focus();
				monthTextBox.SelectionStart = 0;
			}
			if (e.Key == System.Windows.Input.Key.Home)
			{
				dayTextBox.Focus();
				dayTextBox.SelectionStart = 0;
			}
			if (e.Key == System.Windows.Input.Key.End)
			{
				secondTextBox.Focus();
			}
		}

		private void MonthChanged(object sender, TextChangedEventArgs e)
		{
			monthValid = int.TryParse(monthTextBox.Text, out int day) && day <= 12 && day > 0;
			MarkInvalid(PART_monthTextBox, monthTextBox, monthValid);
			if (monthValid && monthTextBox.Text.Length == 2)
			{
				yearTextBox.Focus();
			}
			CalculateDate();
		}

		private void MonthKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Back && monthTextBox.Text.Length == 0)
			{
				dayTextBox.Focus();
			}
			if (e.Key == System.Windows.Input.Key.Delete && monthTextBox.Text.Length == 0)
			{
				yearTextBox.Focus();
				yearTextBox.SelectionStart = 0;
			}
			if (e.Key == System.Windows.Input.Key.Home)
			{
				dayTextBox.Focus();
				dayTextBox.SelectionStart = 0;
			}
			if (e.Key == System.Windows.Input.Key.End)
			{
				secondTextBox.Focus();
			}
		}

		private void YearChanged(object sender, TextChangedEventArgs e)
		{
			yearValid = int.TryParse(yearTextBox.Text, out int year) && year > 0;
			MarkInvalid(PART_yearTextBox, yearTextBox, yearValid);
			if (yearValid && yearTextBox.Text.Length == 4)
			{
				hourTextBox.Focus();
			}
			CalculateDate();
		}

		private void YearKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Back && yearTextBox.Text.Length == 0)
			{
				monthTextBox.Focus();
			}
			if (e.Key == System.Windows.Input.Key.Delete && yearTextBox.Text.Length == 0)
			{
				hourTextBox.Focus();
				hourTextBox.SelectionStart = 0;
			}
			if (e.Key == System.Windows.Input.Key.Home)
			{
				dayTextBox.Focus();
				dayTextBox.SelectionStart = 0;
			}
			if (e.Key == System.Windows.Input.Key.End)
			{
				secondTextBox.Focus();
			}
		}

		private void HourChanged(object sender, TextChangedEventArgs e)
		{
			hourValid = int.TryParse(hourTextBox.Text, out int hour) && hour <= 24 && hour >= 0;
			MarkInvalid(PART_hourTextBox, hourTextBox, hourValid);
			if (hourValid && hourTextBox.Text.Length == 2)
			{
				minuteTextBox.Focus();
			}
			CalculateDate();
		}

		private void HourKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Back && hourTextBox.Text.Length == 0)
			{
				yearTextBox.Focus();
			}
			if (e.Key == System.Windows.Input.Key.Delete && hourTextBox.Text.Length == 0)
			{
				minuteTextBox.Focus();
				minuteTextBox.SelectionStart = 0;
			}
			if (e.Key == System.Windows.Input.Key.Home)
			{
				dayTextBox.Focus();
				dayTextBox.SelectionStart = 0;
			}
			if (e.Key == System.Windows.Input.Key.End)
			{
				secondTextBox.Focus();
			}
		}

		private void MinuteChanged(object sender, TextChangedEventArgs e)
		{
			minuteValid = int.TryParse(minuteTextBox.Text, out int second) && second <= 60 && second >= 0;
			MarkInvalid(PART_minuteTextBox, minuteTextBox, minuteValid);
			if (minuteValid && minuteTextBox.Text.Length == 2)
			{
				secondTextBox.Focus();
			}
			CalculateDate();
		}

		private void MinuteKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Back && minuteTextBox.Text.Length == 0)
			{
				hourTextBox.Focus();
			}
			if (e.Key == System.Windows.Input.Key.Delete && minuteTextBox.Text.Length == 0)
			{
				secondTextBox.Focus();
				secondTextBox.SelectionStart = 0;
			}
			if (e.Key == System.Windows.Input.Key.Home)
			{
				dayTextBox.Focus();
				dayTextBox.SelectionStart = 0;
			}
			if (e.Key == System.Windows.Input.Key.End)
			{
				secondTextBox.Focus();
			}
		}

		private void SecondChanged(object sender, TextChangedEventArgs e)
		{
			secondValid = int.TryParse(secondTextBox.Text, out int day) && day <= 60 && day >= 0;
			MarkInvalid(PART_secondTextBox, secondTextBox, secondValid);
			CalculateDate();
		}

		private void SecondKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Back && secondTextBox.Text.Length == 0)
			{
				minuteTextBox.Focus();
			}
			if (e.Key == System.Windows.Input.Key.Delete && secondTextBox.Text.Length == 0)
			{
				
			}
			if (e.Key == System.Windows.Input.Key.Home)
			{
				dayTextBox.Focus();
				dayTextBox.SelectionStart = 0;
			}
			if (e.Key == System.Windows.Input.Key.End)
			{
				secondTextBox.Focus();
			}
		}

		#endregion Events Handlers

		private void CalculateDate()
		{
			var calculate = dayValid && monthValid && yearValid && hourValid && minuteValid && secondValid;

			if (calculate) DateTime = new DateTime(
				int.Parse(yearTextBox.Text),
				int.Parse(monthTextBox.Text),
				int.Parse(dayTextBox.Text),
				int.Parse(hourTextBox.Text),
				int.Parse(minuteTextBox.Text),
				int.Parse(secondTextBox.Text));
			else DateTime = null;
		}

		private void MarkInvalid(string textBoxName, TextBox textBox, bool valid)
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