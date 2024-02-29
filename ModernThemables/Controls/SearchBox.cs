using System;
using System.Windows;
using System.Windows.Controls;

namespace ModernThemables.Controls
{
	public class SearchBox : Control
	{
		#region Members

		private const string PART_button = "PART_button";

		private ExtendedButton? button;

		#endregion Members

		#region Constructors

		static SearchBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(typeof(SearchBox)));
		}

		#endregion Constructors

		#region Properties

		public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register("SearchText", typeof(string), typeof(SearchBox),
		   new FrameworkPropertyMetadata(null));

		public string SearchText
		{
			get => (string)GetValue(SearchTextProperty);
			set => SetValue(SearchTextProperty, value);
		}

		public double BackgroundOpacity
		{
			get => (double)GetValue(BackgroundOpacityProperty);
			set => SetValue(BackgroundOpacityProperty, value);
		}
		public static readonly DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register(
		  "BackgroundOpacity", typeof(double), typeof(SearchBox), new PropertyMetadata(1d));

		#endregion Properties

		#region Override

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (button != null)
			{
				button.Click -= Button_Click;
			}

			if (Template.FindName(PART_button, this) is ExtendedButton bt)
			{
				button = bt;
			}

			if (button != null)
			{
				button.Click += Button_Click;
			}
			else
			{
				throw new InvalidOperationException("Template missing rquired UI elements");
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			SearchText = string.Empty;
		}

		#endregion Override
	}
}