using CoreUtilities.HelperClasses;
using CoreUtilities.HelperClasses.Extensions;
using CommunityToolkit.Mvvm.Input;
using ModernThemables.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernThemables.Controls
{
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
            ShowSettingsCommand = new RelayCommand(ToggleShowSettings);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(HamburgerMenu),
            new PropertyMetadata(null));

        public DataTemplate SearchItemTemplate
        {
            get => (DataTemplate)GetValue(SearchItemTemplateProperty);
            set => SetValue(SearchItemTemplateProperty, value);
        }
        public static readonly DependencyProperty SearchItemTemplateProperty = DependencyProperty.Register(
            nameof(SearchItemTemplate),
            typeof(DataTemplate),
            typeof(HamburgerMenu),
            new PropertyMetadata(null));

        public Brush AccentBrush
        {
            get => (Brush)GetValue(AccentBrushProperty);
            set => SetValue(AccentBrushProperty, value);
        }
        public static readonly DependencyProperty AccentBrushProperty = DependencyProperty.Register(
            nameof(AccentBrush),
            typeof(Brush),
            typeof(HamburgerMenu),
            new PropertyMetadata(new SolidColorBrush(Colors.DeepSkyBlue)));

        public IEnumerable<IHamburgerMenuItem> Items
        {
            get => (IEnumerable<IHamburgerMenuItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(IEnumerable<IHamburgerMenuItem>),
            typeof(HamburgerMenu),
            new UIPropertyMetadata(new ObservableCollection<IHamburgerMenuItem>()));

        public RangeObservableCollection<IHamburgerMenuItem> FilteredItems
        {
            get => (RangeObservableCollection<IHamburgerMenuItem>)GetValue(FilteredItemsProperty);
            set => SetValue(FilteredItemsProperty, value);
        }
        public static readonly DependencyProperty FilteredItemsProperty = DependencyProperty.Register(
            nameof(FilteredItems),
            typeof(RangeObservableCollection<IHamburgerMenuItem>),
            typeof(HamburgerMenu),
            new FrameworkPropertyMetadata(new RangeObservableCollection<IHamburgerMenuItem>()));

        public bool IsMenuOpen
        {
            get => (bool)GetValue(IsMenuOpenProperty);
            set => SetValue(IsMenuOpenProperty, value);
        }
        public static readonly DependencyProperty IsMenuOpenProperty = DependencyProperty.Register(
            nameof(IsMenuOpen),
            typeof(bool),
            typeof(HamburgerMenu),
            new FrameworkPropertyMetadata(false, OnSetIsMenuOpen));

        public bool IsMenuPinned
        {
            get => (bool)GetValue(IsMenuPinnedProperty);
            set => SetValue(IsMenuPinnedProperty, value);
        }
        public static readonly DependencyProperty IsMenuPinnedProperty = DependencyProperty.Register(
            nameof(IsMenuPinned),
            typeof(bool),
            typeof(HamburgerMenu),
            new FrameworkPropertyMetadata(false));

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
            nameof(SearchText),
            typeof(string),
            typeof(HamburgerMenu),
            new FrameworkPropertyMetadata(string.Empty));

        public FrameworkElement BlurBackground
        {
            get => (FrameworkElement)GetValue(BlurBackgroundProperty);
            set => SetValue(BlurBackgroundProperty, value);
        }

        public static readonly DependencyProperty BlurBackgroundProperty =
            DependencyProperty.Register(
              nameof(BlurBackground),
              typeof(FrameworkElement),
              typeof(HamburgerMenu),
              new PropertyMetadata(default(FrameworkElement)));

        public object SettingsVm
        {
            get => GetValue(SettingsVmProperty);
            set => SetValue(SettingsVmProperty, value);
        }

        public static readonly DependencyProperty SettingsVmProperty =
            DependencyProperty.Register(
              nameof(SettingsVm),
              typeof(object),
              typeof(HamburgerMenu),
              new PropertyMetadata(default(object)));

        public DataTemplate SettingsTemplate
        {
            get => (DataTemplate)GetValue(SettingsTemplateProperty);
            set => SetValue(SettingsTemplateProperty, value);
        }

        public static readonly DependencyProperty SettingsTemplateProperty =
            DependencyProperty.Register(
              nameof(SettingsTemplate),
              typeof(DataTemplate),
              typeof(HamburgerMenu),
              new PropertyMetadata(null));

        public bool ShowSettings
        {
            get => (bool)GetValue(ShowSettingsProperty);
            set => SetValue(ShowSettingsProperty, value);
        }

        public static readonly DependencyProperty ShowSettingsProperty =
            DependencyProperty.Register(
              nameof(ShowSettings),
              typeof(bool),
              typeof(HamburgerMenu),
              new PropertyMetadata(false));

        public ICommand ShowSettingsCommand
        {
            get => (ICommand)GetValue(ShowSettingsCommandProperty);
            set => SetValue(ShowSettingsCommandProperty, value);
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
                if (Items == null) return new RangeObservableCollection<IHamburgerMenuItem>();
                var result = new List<object>(Items);
                Items.ToList().ForEach(x => result.AddRange(x.GetChildren(true)));
                return new RangeObservableCollection<IHamburgerMenuItem>(result.Cast<IHamburgerMenuItem>());
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (openButton != null) openButton.Click -= OpenButton_Click;
            if (Template.FindName(PART_OpenButton, this) is Button2 open) openButton = open;
            if (openButton != null) openButton.Click += OpenButton_Click;

            if (pinButton != null) pinButton.Click -= PinButton_Click;
            if (Template.FindName(PART_PinButton, this) is Button2 pin) pinButton = pin;
            if (pinButton != null) pinButton.Click += PinButton_Click; ;

            if (searchBox != null) searchBox.TextChanged -= SearchBox_SearchTextChanged;
            if (Template.FindName(PART_SearchBox, this) is SearchBox search) searchBox = search;
            if (searchBox != null) searchBox.TextChanged += SearchBox_SearchTextChanged;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            IsMenuOpen = !IsMenuOpen;
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            IsMenuPinned = !IsMenuPinned;
        }

        private void SearchBox_SearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            SearchText = (e.OriginalSource as TextBox)!.Text;
            FilteredItems = AllViewModels
                .Where(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
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
                        this_.searchBox.Text = "";
                    }
                }
            }
        }

        private void ToggleShowSettings()
        {
            if (!IsMenuOpen) IsMenuOpen = true;
            ShowSettings = !ShowSettings;
        }
    }
}