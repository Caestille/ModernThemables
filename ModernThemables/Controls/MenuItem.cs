using CoreUtilities.HelperClasses;
using Microsoft.Toolkit.Mvvm.Input;
using ModernThemables.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernThemables.Controls
{
    public class MenuItem : Control
    {
        static MenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(typeof(MenuItem)));
        }

        public MenuItem()
        {
            ToggleOpenCommand = new RelayCommand(ToggleOpen);
        }

        #region Properties

        public DataTemplate IconTemplate
        {
            get => (DataTemplate)GetValue(IconTemplateProperty);
            set => SetValue(IconTemplateProperty, value);
        }
        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register(
            nameof(IconTemplate),
            typeof(DataTemplate),
            typeof(MenuItem),
            new PropertyMetadata(null));

        public object Icon
        {
            get => (object)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(object),
            typeof(MenuItem),
            new PropertyMetadata(null));

        public DataTemplate TitleTemplate
        {
            get => (DataTemplate)GetValue(TitleTemplateProperty);
            set => SetValue(TitleTemplateProperty, value);
        }
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register(
            nameof(TitleTemplate),
            typeof(DataTemplate),
            typeof(MenuItem),
            new PropertyMetadata(null));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(MenuItem),
            new PropertyMetadata(null));

        public DataTemplate SubTitleTemplate
        {
            get => (DataTemplate)GetValue(SubTitleTemplateProperty);
            set => SetValue(SubTitleTemplateProperty, value);
        }
        public static readonly DependencyProperty SubTitleTemplateProperty = DependencyProperty.Register(
            nameof(SubTitleTemplate),
            typeof(DataTemplate),
            typeof(MenuItem),
            new PropertyMetadata(null));

        public string SubTitle
        {
            get => (string)GetValue(SubTitleProperty);
            set => SetValue(SubTitleProperty, value);
        }
        public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(
            nameof(SubTitle),
            typeof(string),
            typeof(MenuItem),
            new PropertyMetadata(null));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen),
            typeof(bool),
            typeof(MenuItem),
            new PropertyMetadata(false));

        public bool IsContainingMenuOpen
        {
            get => (bool)GetValue(IsContainingMenuOpenProperty);
            set => SetValue(IsContainingMenuOpenProperty, value);
        }
        public static readonly DependencyProperty IsContainingMenuOpenProperty = DependencyProperty.Register(
            nameof(IsContainingMenuOpen),
            typeof(bool),
            typeof(MenuItem),
            new PropertyMetadata(false));

        public bool ReserveIconSpace
        {
            get => (bool)GetValue(ReserveIconSpaceProperty);
            set => SetValue(ReserveIconSpaceProperty, value);
        }
        public static readonly DependencyProperty ReserveIconSpaceProperty = DependencyProperty.Register(
            nameof(ReserveIconSpace),
            typeof(bool),
            typeof(MenuItem),
            new PropertyMetadata(false));

        public RangeObservableCollection<GenericViewModelBase> ChildItems
        {
            get => (RangeObservableCollection<GenericViewModelBase>)GetValue(ChildItemsProperty);
            set => SetValue(ChildItemsProperty, value);
        }
        public static readonly DependencyProperty ChildItemsProperty = DependencyProperty.Register(
            nameof(ChildItems),
            typeof(RangeObservableCollection<GenericViewModelBase>),
            typeof(MenuItem),
            new PropertyMetadata(new RangeObservableCollection<GenericViewModelBase>()));

        public bool CanAddChild
        {
            get => (bool)GetValue(CanAddChildProperty);
            set => SetValue(CanAddChildProperty, value);
        }
        public static readonly DependencyProperty CanAddChildProperty = DependencyProperty.Register(
            nameof(CanAddChild),
            typeof(bool),
            typeof(MenuItem),
            new PropertyMetadata(true));

        public bool CanDelete
        {
            get => (bool)GetValue(CanDeleteProperty);
            set => SetValue(CanDeleteProperty, value);
        }
        public static readonly DependencyProperty CanDeleteProperty = DependencyProperty.Register(
            nameof(CanDelete),
            typeof(bool),
            typeof(MenuItem),
            new PropertyMetadata(true));

        public bool CanOpen
        {
            get => (bool)GetValue(CanOpenProperty);
            set => SetValue(CanOpenProperty, value);
        }
        public static readonly DependencyProperty CanOpenProperty = DependencyProperty.Register(
            nameof(CanOpen),
            typeof(bool),
            typeof(MenuItem),
            new PropertyMetadata(true));

        public ICommand AddChildCommand
        {
            get => (ICommand)GetValue(AddChildCommandProperty);
            set => SetValue(AddChildCommandProperty, value);
        }
        public static readonly DependencyProperty AddChildCommandProperty = DependencyProperty.Register(
            nameof(AddChildCommand),
            typeof(ICommand),
            typeof(MenuItem),
            new PropertyMetadata(null));

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }
        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
            nameof(DeleteCommand),
            typeof(ICommand),
            typeof(MenuItem),
            new PropertyMetadata(null));

        private ICommand ToggleOpenCommand
        {
            get => (ICommand)GetValue(ToggleOpenCommandProperty);
            set => SetValue(ToggleOpenCommandProperty, value);
        }
        public static readonly DependencyProperty ToggleOpenCommandProperty = DependencyProperty.Register(
            nameof(ToggleOpenCommand),
            typeof(ICommand),
            typeof(MenuItem),
            new PropertyMetadata(null));

        #endregion Properties

        private void ToggleOpen()
        {
            IsOpen = !IsOpen;
        }
    }
}