using ModernThemables.Charting.Controls;
using ModernThemables.Charting.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ModernThemables.Controls
{
	public class Menu : Control
	{
		#region Members

		private const string PART_OpenButton = "PART_OpenButton";
		private const string PART_PinButton = "PART_PinButton";
		private const string PART_SearchBox = "PART_SearchBox";

		private ExtendedButton? openButton;
		private ExtendedButton? pinButton;
		private SearchBox? searchBox;

		#endregion Members

		#region Constructors

		static Menu()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Menu), new FrameworkPropertyMetadata(typeof(Menu)));
		}

		#endregion Constructors

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

		public DataTemplate SearchTemplate
		{
			get => (DataTemplate)GetValue(SearchTemplateProperty);
			set => SetValue(SearchTemplateProperty, value);
		}
		public static readonly DependencyProperty SearchTemplateProperty = DependencyProperty.Register(
			nameof(SearchTemplate),
			typeof(DataTemplate),
			typeof(Menu),
			new PropertyMetadata(null));

		public ObservableCollection<object> Items
		{
			get => (ObservableCollection<object>)GetValue(ItemsProperty);
			set => SetValue(ItemsProperty, value);
		}
		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
			nameof(Items),
			typeof(ObservableCollection<object>),
			typeof(Menu),
			new FrameworkPropertyMetadata(new ObservableCollection<object>()));

		public ObservableCollection<object> FilteredItems
		{
			get => (ObservableCollection<object>)GetValue(FilteredItemsProperty);
			set => SetValue(FilteredItemsProperty, value);
		}
		public static readonly DependencyProperty FilteredItemsProperty = DependencyProperty.Register(
			nameof(FilteredItems),
			typeof(ObservableCollection<object>),
			typeof(Menu),
			new FrameworkPropertyMetadata(new ObservableCollection<object>()));

		public bool IsMenuOpen
		{
			get => (bool)GetValue(IsMenuOpenProperty);
			set => SetValue(IsMenuOpenProperty, value);
		}
		public static readonly DependencyProperty IsMenuOpenProperty = DependencyProperty.Register(
			nameof(IsMenuOpen),
			typeof(bool),
			typeof(Menu),
			new FrameworkPropertyMetadata(false));

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

		#endregion Properties

		#region Override

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (openButton != null) { openButton.Click -= OpenButton_Click; }
			if (Template.FindName(PART_OpenButton, this) is ExtendedButton bt) { openButton = bt; }
			if (openButton != null) { openButton.Click += OpenButton_Click; }
			else { throw new InvalidOperationException("Template missing required UI elements"); }
		}

		private void OpenButton_Click(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		#endregion Override
	}
}