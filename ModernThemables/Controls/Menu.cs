using CoreUtilities.HelperClasses;
using CoreUtilities.HelperClasses.Extensions;
using Microsoft.Toolkit.Mvvm.Input;
using ModernThemables.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernThemables.Controls
{
    public class Menu : Control
    {
        private const string PART_OpenButton = "PART_OpenButton";
        private const string PART_PinButton = "PART_PinButton";
        private const string PART_SearchBox = "PART_SearchBox";

        private Button2? openButton;
        private Button2? pinButton;
        private SearchBox? searchBox;

        static Menu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Menu), new FrameworkPropertyMetadata(typeof(Menu)));
        }

        public Menu()
        {
            ShowSettingsCommand = new RelayCommand(ToggleShowSettings);
        }

        #region Properties

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(Menu),
            new PropertyMetadata(null));

        public DataTemplate SearchItemTemplate
        {
            get => (DataTemplate)GetValue(SearchItemTemplateProperty);
            set => SetValue(SearchItemTemplateProperty, value);
        }
        public static readonly DependencyProperty SearchItemTemplateProperty = DependencyProperty.Register(
            nameof(SearchItemTemplate),
            typeof(DataTemplate),
            typeof(Menu),
            new PropertyMetadata(null));

        public IEnumerable<IMenuItem> Items
        {
            get => (IEnumerable<IMenuItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(IEnumerable<IMenuItem>),
            typeof(Menu),
            new UIPropertyMetadata(new ObservableCollection<IMenuItem>()));

        public RangeObservableCollection<GenericViewModelBase> FilteredItems
        {
            get => (RangeObservableCollection<GenericViewModelBase>)GetValue(FilteredItemsProperty);
            set => SetValue(FilteredItemsProperty, value);
        }
        public static readonly DependencyProperty FilteredItemsProperty = DependencyProperty.Register(
            nameof(FilteredItems),
            typeof(RangeObservableCollection<GenericViewModelBase>),
            typeof(Menu),
            new FrameworkPropertyMetadata(new RangeObservableCollection<GenericViewModelBase>()));

        public bool IsMenuOpen
        {
            get => (bool)GetValue(IsMenuOpenProperty);
            set => SetValue(IsMenuOpenProperty, value);
        }
        public static readonly DependencyProperty IsMenuOpenProperty = DependencyProperty.Register(
            nameof(IsMenuOpen),
            typeof(bool),
            typeof(Menu),
            new FrameworkPropertyMetadata(false, OnSetIsMenuOpen));

        public bool IsMenuPinned
        {
            get => (bool)GetValue(IsMenuPinnedProperty);
            set => SetValue(IsMenuPinnedProperty, value);
        }
        public static readonly DependencyProperty IsMenuPinnedProperty = DependencyProperty.Register(
            nameof(IsMenuPinned),
            typeof(bool),
            typeof(Menu),
            new FrameworkPropertyMetadata(false));

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
            nameof(SearchText),
            typeof(string),
            typeof(Menu),
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
              typeof(Menu),
              new PropertyMetadata(default(FrameworkElement)));

        public object SettingsVm
        {
            get => (object)GetValue(SettingsVmProperty);
            set => SetValue(SettingsVmProperty, value);
        }

        public static readonly DependencyProperty SettingsVmProperty =
            DependencyProperty.Register(
              nameof(SettingsVm),
              typeof(object),
              typeof(Menu),
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
              typeof(Menu),
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
              typeof(Menu),
              new PropertyMetadata(false));

        private ICommand ShowSettingsCommand
        {
            get => (ICommand)GetValue(ShowSettingsCommandProperty);
            set => SetValue(ShowSettingsCommandProperty, value);
        }

        public static readonly DependencyProperty ShowSettingsCommandProperty =
            DependencyProperty.Register(
              nameof(ShowSettingsCommand),
              typeof(ICommand),
              typeof(Menu),
              new PropertyMetadata(null));

        public RangeObservableCollection<GenericViewModelBase> AllViewModels
        {
            get
            {
                if (Items == null) return new RangeObservableCollection<GenericViewModelBase>();
                var result = new List<object>(Items);
                Items.ToList().ForEach(x => result.AddRange(x.GetChildren(true)));
                return new RangeObservableCollection<GenericViewModelBase>(result.Cast<GenericViewModelBase>());
            }
        }

        #endregion Properties

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (openButton != null) openButton.Click -= OpenButton_Click;
            if (Template.FindName(PART_OpenButton, this) is Button2 open) openButton = open;
            if (openButton != null) openButton.Click += OpenButton_Click;

            if (pinButton != null) pinButton.Click -= PinButton_Click;
            if (Template.FindName(PART_PinButton, this) is Button2 pin) pinButton = pin;
            if (pinButton != null) pinButton.Click += PinButton_Click; ;

            if (searchBox != null) searchBox.SearchTextChanged -= SearchBox_SearchTextChanged;
            if (Template.FindName(PART_SearchBox, this) is SearchBox search) searchBox = search;
            if (searchBox != null) searchBox.SearchTextChanged += SearchBox_SearchTextChanged;
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            IsMenuOpen = !IsMenuOpen;
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            IsMenuPinned = !IsMenuPinned;
        }

        private void SearchBox_SearchTextChanged(object? sender, string e)
        {
            SearchText = e;
            FilteredItems = AllViewModels
                .Where(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToRangeObservableCollection();
        }

        private static void OnSetIsMenuOpen(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Menu this_)
            {
                if (!this_.IsMenuOpen)
                {
                    if (this_.ShowSettings)
                    {
                        this_.ShowSettings = false;
                    }

                    if (this_.searchBox != null)
                    {
                        this_.searchBox.SearchText = "";
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