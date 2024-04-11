using System;
using System.Windows;
using System.Windows.Controls;

namespace ModernThemables.Controls
{
    public class SearchBox : Control
	{
		private const string PART_button = "PART_button";

		private Button2? button;

		public event EventHandler<string>? SearchTextChanged;

		static SearchBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(typeof(SearchBox)));
		}

		#region Properties

		public string SearchText
		{
			get => (string)GetValue(SearchTextProperty);
			set => SetValue(SearchTextProperty, value);
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(
                nameof(SearchText),
                typeof(string),
                typeof(SearchBox),
                new FrameworkPropertyMetadata(null, OnSearchTextChanged));

        public double BackgroundOpacity
		{
			get => (double)GetValue(BackgroundOpacityProperty);
			set => SetValue(BackgroundOpacityProperty, value);
		}

		public static readonly DependencyProperty BackgroundOpacityProperty =
            DependencyProperty.Register(
		        nameof(BackgroundOpacity),
                typeof(double),
                typeof(SearchBox),
                new PropertyMetadata(1d));

		#endregion Properties

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (button != null)
			{
				button.Click -= Button_Click;
			}

			if (Template.FindName(PART_button, this) is Button2 bt)
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

        private static void OnSearchTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is SearchBox this_)
            {
                this_.SearchTextChanged?.Invoke(this_, (string)e.NewValue);
            }
        }
    }
}