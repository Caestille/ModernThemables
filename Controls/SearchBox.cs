using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernThemables.Controls
{
	[TemplatePart(Name = PART_textBox, Type = typeof(TextBox))]
	[TemplatePart(Name = PART_textBlock, Type = typeof(TextBlock))]

	public class SearchBox : Control
	{
		#region Members

		private const string PART_textBox = "PART_textBox";
		private const string PART_textBlock = "PART_textBlock";
		private const string PART_button = "PART_button";

		private TextBox textBox;
		private TextBlock textBlock;
		private ExtendedButton button;

		#endregion Members

		#region Constructors

		static SearchBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(typeof(SearchBox)));
		}

		public SearchBox()
		{
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
			get { return (double)GetValue(BackgroundOpacityProperty); }
			set { SetValue(BackgroundOpacityProperty, value); }
		}
		public static readonly DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register(
		  "BackgroundOpacity", typeof(double), typeof(SearchBox), new PropertyMetadata(1d));

		#endregion Properties

		#region Override

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (textBox != null)
			{
				//textBox.TextChanged -= TextChanged;
				textBox.PreviewKeyDown -= KeyDown;
			}
			textBox = this.Template.FindName(PART_textBox, this) as TextBox;
			if (textBox != null)
			{
				//textBox.TextChanged += TextChanged;
				textBox.PreviewKeyDown += KeyDown;
			}

			textBlock = this.Template.FindName(PART_textBlock, this) as TextBlock;

			if (button != null)
			{
				button.Click -= Button_Click;
			}
			button = this.Template.FindName(PART_button, this) as ExtendedButton;
			if (button != null)
			{
				button.Click += Button_Click;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.SearchText = string.Empty;
		}

		#endregion Override

		#region Events Handlers

		private void KeyDown(object sender, KeyEventArgs e)
		{
			
		}

		#endregion Events Handlers
	}
}