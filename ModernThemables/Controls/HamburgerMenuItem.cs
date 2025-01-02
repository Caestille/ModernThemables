using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernThemables.Controls
{
    public class HamburgerMenuItem : Control
    {
        readonly static SolidColorBrush DefaultMouseOverProperty = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFBEE6FD")!;

        public HamburgerMenuItem()
        {
            InternalSelectCommand = new RelayCommand(Select);
            if (StartOpen)
            {
                IsOpen = true;
            }
        }

        public Brush SelectedForeground
        {
            get => (Brush)GetValue(SelectedForegroundProperty);
            set => SetValue(SelectedForegroundProperty, value);
        }
        public static readonly DependencyProperty SelectedForegroundProperty = DependencyProperty.Register(
            nameof(SelectedForeground),
            typeof(Brush),
            typeof(HamburgerMenuItem),
            new FrameworkPropertyMetadata(DefaultMouseOverProperty));

        public Brush SubtitleForeground
        {
            get => (Brush)GetValue(SubtitleForegroundProperty);
            set => SetValue(SubtitleForegroundProperty, value);
        }
        public static readonly DependencyProperty SubtitleForegroundProperty = DependencyProperty.Register(
            nameof(SubtitleForeground),
            typeof(Brush),
            typeof(HamburgerMenuItem),
            new FrameworkPropertyMetadata(DefaultMouseOverProperty));

        public Brush MouseOverBrush
        {
            get => (Brush)GetValue(MouseOverBrushProperty);
            set => SetValue(MouseOverBrushProperty, value);
        }
        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(
            nameof(MouseOverBrush),
            typeof(Brush),
            typeof(HamburgerMenuItem),
            new FrameworkPropertyMetadata(DefaultMouseOverProperty));

        public Brush MouseDownBrush
        {
            get => (Brush)GetValue(MouseDownBrushProperty);
            set => SetValue(MouseDownBrushProperty, value);
        }
        public static readonly DependencyProperty MouseDownBrushProperty = DependencyProperty.Register(
            nameof(MouseDownBrush),
            typeof(Brush),
            typeof(HamburgerMenuItem));

        public Brush AccentBrush
        {
            get => (Brush)GetValue(AccentBrushProperty);
            set => SetValue(AccentBrushProperty, value);
        }
        public static readonly DependencyProperty AccentBrushProperty = DependencyProperty.Register(
            nameof(AccentBrush),
            typeof(Brush),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(new SolidColorBrush(Colors.DeepSkyBlue)));

        public DataTemplate IconTemplate
        {
            get => (DataTemplate)GetValue(IconTemplateProperty);
            set => SetValue(IconTemplateProperty, value);
        }
        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register(
            nameof(IconTemplate),
            typeof(DataTemplate),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(null));

        public DataTemplate ChildItemTemplate
        {
            get => (DataTemplate)GetValue(ChildItemTemplateProperty);
            set => SetValue(ChildItemTemplateProperty, value);
        }
        public static readonly DependencyProperty ChildItemTemplateProperty = DependencyProperty.Register(
            nameof(ChildItemTemplate),
            typeof(DataTemplate),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(null));

        public DataTemplate ChildItemsTemplate
        {
            get => (DataTemplate)GetValue(ChildItemsTemplateProperty);
            set => SetValue(ChildItemsTemplateProperty, value);
        }
        public static readonly DependencyProperty ChildItemsTemplateProperty = DependencyProperty.Register(
            nameof(ChildItemsTemplate),
            typeof(DataTemplate),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(null));

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(object),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(null));

        public DataTemplate TitleTemplate
        {
            get => (DataTemplate)GetValue(TitleTemplateProperty);
            set => SetValue(TitleTemplateProperty, value);
        }
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register(
            nameof(TitleTemplate),
            typeof(DataTemplate),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(null));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(null));

        public DataTemplate SubTitleTemplate
        {
            get => (DataTemplate)GetValue(SubTitleTemplateProperty);
            set => SetValue(SubTitleTemplateProperty, value);
        }
        public static readonly DependencyProperty SubTitleTemplateProperty = DependencyProperty.Register(
            nameof(SubTitleTemplate),
            typeof(DataTemplate),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(null));

        public string SubTitle
        {
            get => (string)GetValue(SubTitleProperty);
            set => SetValue(SubTitleProperty, value);
        }
        public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(
            nameof(SubTitle),
            typeof(string),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(null));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen),
            typeof(bool),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(false));

        public bool StartOpen
        {
            get => (bool)GetValue(StartOpenProperty);
            set => SetValue(StartOpenProperty, value);
        }
        public static readonly DependencyProperty StartOpenProperty = DependencyProperty.Register(
            nameof(StartOpen),
            typeof(bool),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(false, OnSetStartOpen));

        public bool ShowOpenIndicator
        {
            get => (bool)GetValue(ShowOpenIndicatorProperty);
            set => SetValue(ShowOpenIndicatorProperty, value);
        }
        public static readonly DependencyProperty ShowOpenIndicatorProperty = DependencyProperty.Register(
            nameof(ShowOpenIndicator),
            typeof(bool),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(true));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(false));

        public bool IsContainingMenuOpen
        {
            get => (bool)GetValue(IsContainingMenuOpenProperty);
            set => SetValue(IsContainingMenuOpenProperty, value);
        }
        public static readonly DependencyProperty IsContainingMenuOpenProperty = DependencyProperty.Register(
            nameof(IsContainingMenuOpen),
            typeof(bool),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(true));

        public bool ReserveIconSpace
        {
            get => (bool)GetValue(ReserveIconSpaceProperty);
            set => SetValue(ReserveIconSpaceProperty, value);
        }
        public static readonly DependencyProperty ReserveIconSpaceProperty = DependencyProperty.Register(
            nameof(ReserveIconSpace),
            typeof(bool),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(false));

        public IEnumerable<object> ChildItems
        {
            get => (IEnumerable<object>)GetValue(ChildItemsProperty);
            set => SetValue(ChildItemsProperty, value);
        }
        public static readonly DependencyProperty ChildItemsProperty = DependencyProperty.Register(
            nameof(ChildItems),
            typeof(IEnumerable<object>),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(new ObservableCollection<object>()));

        public bool CanAddChild
        {
            get => (bool)GetValue(CanAddChildProperty);
            set => SetValue(CanAddChildProperty, value);
        }
        public static readonly DependencyProperty CanAddChildProperty = DependencyProperty.Register(
            nameof(CanAddChild),
            typeof(bool),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(true));

        public bool CanDelete
        {
            get => (bool)GetValue(CanDeleteProperty);
            set => SetValue(CanDeleteProperty, value);
        }
        public static readonly DependencyProperty CanDeleteProperty = DependencyProperty.Register(
            nameof(CanDelete),
            typeof(bool),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(true));

        public bool CanOpen
        {
            get => (bool)GetValue(CanOpenProperty);
            set => SetValue(CanOpenProperty, value);
        }
        public static readonly DependencyProperty CanOpenProperty = DependencyProperty.Register(
            nameof(CanOpen),
            typeof(bool),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(true));

        public ICommand AddChildCommand
        {
            get => (ICommand)GetValue(AddChildCommandProperty);
            set => SetValue(AddChildCommandProperty, value);
        }
        public static readonly DependencyProperty AddChildCommandProperty = DependencyProperty.Register(
            nameof(AddChildCommand),
            typeof(ICommand),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(null));

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }
        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
            nameof(DeleteCommand),
            typeof(ICommand),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(null));

        public ICommand SelectCommand
        {
            get => (ICommand)GetValue(SelectCommandProperty);
            set => SetValue(SelectCommandProperty, value);
        }
        public static readonly DependencyProperty SelectCommandProperty = DependencyProperty.Register(
            nameof(SelectCommand),
            typeof(ICommand),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(null));

        internal ICommand InternalSelectCommand
        {
            get => (ICommand)GetValue(InternalSelectCommandProperty);
            set => SetValue(InternalSelectCommandProperty, value);
        }
        public static readonly DependencyProperty InternalSelectCommandProperty = DependencyProperty.Register(
            nameof(InternalSelectCommand),
            typeof(ICommand),
            typeof(HamburgerMenuItem),
            new PropertyMetadata(null));
        
        private static void OnSetStartOpen(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is HamburgerMenuItem this_ && this_.StartOpen && !this_.IsOpen)
            {
                this_.IsOpen = true;
            }
        }


        private void Select()
        {
            if (ChildItems.Any() && CanOpen)
            {
                IsOpen = !IsOpen;
                return;
            }

            if (SelectCommand != null)
            {
                SelectCommand.Execute(this);
            }
        }
    }
}