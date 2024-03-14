using CoreUtilities.HelperClasses;
using CoreUtilities.HelperClasses.Extensions;
using ModernThemables.Charting.Controls;
using ModernThemables.Charting.Interfaces;
using ModernThemables.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ModernThemables.Controls
{
	public class Menu : Control
	{
		private const string PART_OpenButton = "PART_OpenButton";
		private const string PART_PinButton = "PART_PinButton";
		private const string PART_SearchBox = "PART_SearchBox";

		private ExtendedButton? openButton;
		private ExtendedButton? pinButton;
		private SearchBox? searchBox;

		static Menu()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Menu), new FrameworkPropertyMetadata(typeof(Menu)));
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

		public ObservableCollection<GenericViewModelBase> Items
		{
			get => (ObservableCollection<GenericViewModelBase>)GetValue(ItemsProperty);
			set => SetValue(ItemsProperty, value);
		}
		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
			nameof(Items),
			typeof(ObservableCollection<GenericViewModelBase>),
			typeof(Menu),
			new FrameworkPropertyMetadata(new ObservableCollection<GenericViewModelBase>()));

		public ObservableCollection<GenericViewModelBase> FilteredItems
		{
			get => (ObservableCollection<GenericViewModelBase>)GetValue(FilteredItemsProperty);
			set => SetValue(FilteredItemsProperty, value);
		}
		public static readonly DependencyProperty FilteredItemsProperty = DependencyProperty.Register(
			nameof(FilteredItems),
			typeof(ObservableCollection<GenericViewModelBase>),
			typeof(Menu),
			new FrameworkPropertyMetadata(new ObservableCollection<GenericViewModelBase>()));

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

        public RangeObservableCollection<GenericViewModelBase> AllViewModels
        {
            get
            {
                var result = new RangeObservableCollection<GenericViewModelBase>(Items);
                Items.ToList().ForEach(x => x.GetChildren(ref result, true));
                return result;
            }
        }

        #endregion Properties

        public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (openButton != null) openButton.Click -= OpenButton_Click;
			if (Template.FindName(PART_OpenButton, this) is ExtendedButton open) openButton = open;
			if (openButton != null) openButton.Click += OpenButton_Click;

			if (pinButton != null) pinButton.Click -= PinButton_Click;
			if (Template.FindName(PART_PinButton, this) is ExtendedButton pin) pinButton = pin;
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
				.ToObservableCollection();
        }

        private static void OnSetIsMenuOpen(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Menu this_)
            {
                if (!this_.IsMenuOpen && this_.searchBox != null)
                {
                    this_.searchBox.SearchText = "";
                }

                foreach (var vm in this_.AllViewModels)
                {
                    vm.IsMenuItemExpanded = this_.IsMenuOpen;
                }
            }
        }
    }
}