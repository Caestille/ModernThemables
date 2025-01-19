namespace ModernThemables.Controls;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using CoreUtilities.Helpers.Extensions;
using CoreUtilities.Helpers.WPF;
using ModernThemables.ViewModels;

public class HamburgerMenu : Control
{
    private const string PART_OpenButton = "PART_OpenButton";
    private const string PART_PinButton = "PART_PinButton";
    private const string PART_SearchBox = "PART_SearchBox";

    private Button2? openButton;
    private Button2? pinButton;
    private SearchBox? searchBox;

    public HamburgerMenu()
    {
        this.ShowSettingsCommand = new RelayCommand(this.ToggleShowSettings);
    }

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)this.GetValue(ItemTemplateProperty);
        set => this.SetValue(ItemTemplateProperty, value);
    }

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(HamburgerMenu),
        new PropertyMetadata(null));

    public DataTemplate SearchItemTemplate
    {
        get => (DataTemplate)this.GetValue(SearchItemTemplateProperty);
        set => this.SetValue(SearchItemTemplateProperty, value);
    }

    public static readonly DependencyProperty SearchItemTemplateProperty = DependencyProperty.Register(
        nameof(SearchItemTemplate),
        typeof(DataTemplate),
        typeof(HamburgerMenu),
        new PropertyMetadata(null));

    public Brush AccentBrush
    {
        get => (Brush)this.GetValue(AccentBrushProperty);
        set => this.SetValue(AccentBrushProperty, value);
    }

    public static readonly DependencyProperty AccentBrushProperty = DependencyProperty.Register(
        nameof(AccentBrush),
        typeof(Brush),
        typeof(HamburgerMenu),
        new PropertyMetadata(new SolidColorBrush(Colors.DeepSkyBlue)));

    public IEnumerable<IHamburgerMenuItem> Items
    {
        get => (IEnumerable<IHamburgerMenuItem>)this.GetValue(ItemsProperty);
        set => this.SetValue(ItemsProperty, value);
    }

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        nameof(Items),
        typeof(IEnumerable<IHamburgerMenuItem>),
        typeof(HamburgerMenu),
        new UIPropertyMetadata(new ObservableCollection<IHamburgerMenuItem>()));

    public RangeObservableCollection<IHamburgerMenuItem> FilteredItems
    {
        get => (RangeObservableCollection<IHamburgerMenuItem>)this.GetValue(FilteredItemsProperty);
        set => this.SetValue(FilteredItemsProperty, value);
    }

    public static readonly DependencyProperty FilteredItemsProperty = DependencyProperty.Register(
        nameof(FilteredItems),
        typeof(RangeObservableCollection<IHamburgerMenuItem>),
        typeof(HamburgerMenu),
        new FrameworkPropertyMetadata(new RangeObservableCollection<IHamburgerMenuItem>()));

    public bool IsMenuOpen
    {
        get => (bool)this.GetValue(IsMenuOpenProperty);
        set => this.SetValue(IsMenuOpenProperty, value);
    }

    public static readonly DependencyProperty IsMenuOpenProperty = DependencyProperty.Register(
        nameof(IsMenuOpen),
        typeof(bool),
        typeof(HamburgerMenu),
        new FrameworkPropertyMetadata(false, OnSetIsMenuOpen));

    public bool IsMenuPinned
    {
        get => (bool)this.GetValue(IsMenuPinnedProperty);
        set => this.SetValue(IsMenuPinnedProperty, value);
    }

    public static readonly DependencyProperty IsMenuPinnedProperty = DependencyProperty.Register(
        nameof(IsMenuPinned),
        typeof(bool),
        typeof(HamburgerMenu),
        new FrameworkPropertyMetadata(false));

    public string SearchText
    {
        get => (string)this.GetValue(SearchTextProperty);
        set => this.SetValue(SearchTextProperty, value);
    }

    public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
        nameof(SearchText),
        typeof(string),
        typeof(HamburgerMenu),
        new FrameworkPropertyMetadata(string.Empty));

    public FrameworkElement BlurBackground
    {
        get => (FrameworkElement)this.GetValue(BlurBackgroundProperty);
        set => this.SetValue(BlurBackgroundProperty, value);
    }

    public static readonly DependencyProperty BlurBackgroundProperty =
        DependencyProperty.Register(
          nameof(BlurBackground),
          typeof(FrameworkElement),
          typeof(HamburgerMenu),
          new PropertyMetadata(default(FrameworkElement)));

    public object SettingsVm
    {
        get => this.GetValue(SettingsVmProperty);
        set => this.SetValue(SettingsVmProperty, value);
    }

    public static readonly DependencyProperty SettingsVmProperty =
        DependencyProperty.Register(
          nameof(SettingsVm),
          typeof(object),
          typeof(HamburgerMenu),
          new PropertyMetadata(default(object)));

    public DataTemplate SettingsTemplate
    {
        get => (DataTemplate)this.GetValue(SettingsTemplateProperty);
        set => this.SetValue(SettingsTemplateProperty, value);
    }

    public static readonly DependencyProperty SettingsTemplateProperty =
        DependencyProperty.Register(
          nameof(SettingsTemplate),
          typeof(DataTemplate),
          typeof(HamburgerMenu),
          new PropertyMetadata(null));

    public bool ShowSettings
    {
        get => (bool)this.GetValue(ShowSettingsProperty);
        set => this.SetValue(ShowSettingsProperty, value);
    }

    public static readonly DependencyProperty ShowSettingsProperty =
        DependencyProperty.Register(
          nameof(ShowSettings),
          typeof(bool),
          typeof(HamburgerMenu),
          new PropertyMetadata(false));

    public ICommand ShowSettingsCommand
    {
        get => (ICommand)this.GetValue(ShowSettingsCommandProperty);
        set => this.SetValue(ShowSettingsCommandProperty, value);
    }

    public static readonly DependencyProperty ShowSettingsCommandProperty =
        DependencyProperty.Register(
          nameof(ShowSettingsCommand),
          typeof(ICommand),
          typeof(HamburgerMenu),
          new PropertyMetadata(null));

    public RangeObservableCollection<IHamburgerMenuItem> AllViewModels
    {
        get
        {
            if (this.Items == null)
            {
                return new RangeObservableCollection<IHamburgerMenuItem>();
            }

            var result = new List<object>(this.Items);
            this.Items.ToList().ForEach(x => result.AddRange(x.GetChildren(true)));
            return new RangeObservableCollection<IHamburgerMenuItem>(result.Cast<IHamburgerMenuItem>());
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (this.openButton != null)
        {
            this.openButton.Click -= this.OpenButton_Click;
        }

        if (this.Template.FindName(PART_OpenButton, this) is Button2 open)
        {
            this.openButton = open;
        }

        if (this.openButton != null)
        {
            this.openButton.Click += this.OpenButton_Click;
        }

        if (this.pinButton != null)
        {
            this.pinButton.Click -= this.PinButton_Click;
        }

        if (this.Template.FindName(PART_PinButton, this) is Button2 pin)
        {
            this.pinButton = pin;
        }

        if (this.pinButton != null)
        {
            this.pinButton.Click += this.PinButton_Click;
        }

        ;

        if (this.searchBox != null)
        {
            this.searchBox.TextChanged -= this.SearchBox_SearchTextChanged;
        }

        if (this.Template.FindName(PART_SearchBox, this) is SearchBox search)
        {
            this.searchBox = search;
        }

        if (this.searchBox != null)
        {
            this.searchBox.TextChanged += this.SearchBox_SearchTextChanged;
        }
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e) => this.IsMenuOpen = !this.IsMenuOpen;

    private void PinButton_Click(object sender, RoutedEventArgs e) => this.IsMenuPinned = !this.IsMenuPinned;

    private void SearchBox_SearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        this.SearchText = (e.OriginalSource as TextBox)!.Text;
        this.FilteredItems = this.AllViewModels
            .Where(x => x.Name.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase))
            .ToRangeObservableCollection();
    }

    private static void OnSetIsMenuOpen(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is HamburgerMenu this_)
        {
            if (!this_.IsMenuOpen)
            {
                if (this_.ShowSettings)
                {
                    this_.ShowSettings = false;
                }

                if (this_.searchBox != null)
                {
                    this_.searchBox.Text = string.Empty;
                }
            }
        }
    }

    private void ToggleShowSettings()
    {
        if (!this.IsMenuOpen)
        {
            this.IsMenuOpen = true;
        }

        this.ShowSettings = !this.ShowSettings;
    }
}
