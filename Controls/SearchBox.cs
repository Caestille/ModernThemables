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

		private TextBox textBox;
		private TextBlock textBlock;

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
		   new FrameworkPropertyMetadata(null, OnSetSearchText));

		public string SearchText
		{
			get => (string)GetValue(SearchTextProperty);
			set => SetValue(SearchTextProperty, value);
		}

		private static void OnSetSearchText(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{

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
				textBox.TextChanged -= TextChanged;
				textBox.PreviewKeyDown -= KeyDown;
			}
			textBox = this.Template.FindName(PART_textBox, this) as TextBox;
			if (textBox != null)
			{
				textBox.TextChanged += TextChanged;
				textBox.PreviewKeyDown += KeyDown;
			}

			if (textBlock != null)
			{
				
			}
			textBlock = this.Template.FindName(PART_textBlock, this) as TextBlock;
			if (textBox != null)
			{
				
			}
		}

		#endregion Override

		#region Methods



		#endregion

		#region Events



		#endregion

		#region Events Handlers

		private void TextChanged(object sender, TextChangedEventArgs e)
		{
			SearchText = textBox.Text;
		}

		private void KeyDown(object sender, KeyEventArgs e)
		{
			
		}

		#endregion Events Handlers
	}
}