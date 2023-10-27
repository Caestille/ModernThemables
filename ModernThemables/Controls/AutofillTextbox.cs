using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Windows.Input;

namespace ModernThemables.Controls
{
	public class AutofillTextbox : TextBox
	{
		#region Members

		#endregion Members

		#region Constructors

		static AutofillTextbox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AutofillTextbox), new FrameworkPropertyMetadata(typeof(AutofillTextbox)));
		}

		public AutofillTextbox()
		{
			
		}

		#endregion Constructors

		#region Properties

		public ObservableCollection<string> AutofillOptions
		{
			get => (ObservableCollection<string>)GetValue(AutofillOptionsProperty);
			set => SetValue(AutofillOptionsProperty, value);
		}

		public static readonly DependencyProperty AutofillOptionsProperty = DependencyProperty.Register(
			nameof(AutofillOptions),
			typeof(ObservableCollection<string>),
			typeof(AutofillTextbox),
			new FrameworkPropertyMetadata(null));

		public string SuggestionText
		{
			get => (string)GetValue(SuggestionTextProperty);
			set => SetValue(SuggestionTextProperty, value);
		}

		public static readonly DependencyProperty SuggestionTextProperty = DependencyProperty.Register(
			nameof(SuggestionText),
			typeof(string),
			typeof(AutofillTextbox),
			new FrameworkPropertyMetadata(string.Empty));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius",
            typeof(CornerRadius),
            typeof(AutofillTextbox),
            new PropertyMetadata(new CornerRadius(0)));

        #endregion Properties

        #region Override

        protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Tab && !string.IsNullOrWhiteSpace(SuggestionText))
			{
				Text = SuggestionText;
				SelectionStart = Text.Length;
				SelectionLength = 0;
				SuggestionText = string.Empty;
				e.Handled = true;
			}
			else if (e.Key == Key.Escape)
			{
				SuggestionText = string.Empty;
				e.Handled = true;
			}
			else if (e.Key == Key.Up && AutofillOptions.Contains(SuggestionText))
			{
				var index = AutofillOptions.IndexOf(SuggestionText) - 1;
				if (index == -1) index = AutofillOptions.Count - 1;
				SuggestionText = AutofillOptions[index];
                e.Handled = true;
			}
			else if (e.Key == Key.Down && AutofillOptions.Contains(SuggestionText))
            {
                var index = AutofillOptions.IndexOf(SuggestionText) + 1;
                if (index == AutofillOptions.Count) index = 0;
                SuggestionText = AutofillOptions[index];
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			base.OnTextChanged(e);

			if (AutofillOptions == null || !AutofillOptions.Any() || SuggestionText.Any(Text.Contains))
			{
				return;
			}

			SuggestionText = string.IsNullOrWhiteSpace(Text) ? string.Empty : AutofillOptions.FirstOrDefault(x => x.StartsWith(Text, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}

		#endregion Override
	}
}