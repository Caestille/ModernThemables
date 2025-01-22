namespace ModernThemables.Controls;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

public class AutofillTextbox : TextBox
{
    public static readonly DependencyProperty AutofillOptionsProperty = DependencyProperty.Register(
        nameof(AutofillOptions),
        typeof(ObservableCollection<string>),
        typeof(AutofillTextbox),
        new FrameworkPropertyMetadata(null));

    public static readonly DependencyProperty SuggestionTextProperty = DependencyProperty.Register(
        nameof(SuggestionText),
        typeof(string),
        typeof(AutofillTextbox),
        new FrameworkPropertyMetadata(string.Empty));

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(AutofillTextbox),
        new PropertyMetadata(new CornerRadius(0)));

    public static readonly DependencyProperty WatermarkForegroundProperty = DependencyProperty.Register(
        nameof(WatermarkForeground),
        typeof(Brush),
        typeof(AutofillTextbox));

    static AutofillTextbox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AutofillTextbox), new FrameworkPropertyMetadata(typeof(AutofillTextbox)));
    }

    public AutofillTextbox()
    {
    }

    public ObservableCollection<string> AutofillOptions
    {
        get => (ObservableCollection<string>)this.GetValue(AutofillOptionsProperty);
        set => this.SetValue(AutofillOptionsProperty, value);
    }

    public string SuggestionText
    {
        get => (string)this.GetValue(SuggestionTextProperty);
        set => this.SetValue(SuggestionTextProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)this.GetValue(CornerRadiusProperty);
        set => this.SetValue(CornerRadiusProperty, value);
    }

    public Brush WatermarkForeground
    {
        get => (Brush)this.GetValue(WatermarkForegroundProperty);
        set => this.SetValue(WatermarkForegroundProperty, value);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Tab && !string.IsNullOrWhiteSpace(this.SuggestionText))
        {
            this.Text = this.SuggestionText;
            this.SelectionStart = this.Text.Length;
            this.SelectionLength = 0;
            this.SuggestionText = string.Empty;
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            this.SuggestionText = string.Empty;
            e.Handled = true;
        }
        else if (e.Key == Key.Up && this.AutofillOptions.Contains(this.SuggestionText))
        {
            var index = this.AutofillOptions.IndexOf(this.SuggestionText) - 1;
            if (index == -1)
            {
                index = this.AutofillOptions.Count - 1;
            }

            this.SuggestionText = this.AutofillOptions[index];
            e.Handled = true;
        }
        else if (e.Key == Key.Down && this.AutofillOptions.Contains(this.SuggestionText))
        {
            var index = this.AutofillOptions.IndexOf(this.SuggestionText) + 1;
            if (index == this.AutofillOptions.Count)
            {
                index = 0;
            }

            this.SuggestionText = this.AutofillOptions[index];
            e.Handled = true;
        }

        base.OnPreviewKeyDown(e);
    }

    protected override void OnTextChanged(TextChangedEventArgs e)
    {
        base.OnTextChanged(e);

        if (this.AutofillOptions == null || !this.AutofillOptions.Any())
        {
            return;
        }

        this.SuggestionText = string.IsNullOrWhiteSpace(this.Text) ? string.Empty : this.AutofillOptions.FirstOrDefault(x => x.StartsWith(this.Text, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
    }
}
