namespace ModernThemables.Controls;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;

public class HamburgerMenuItem : Control
{
    readonly static SolidColorBrush DefaultMouseOverProperty = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFBEE6FD")!;

    public HamburgerMenuItem()
    {
        this.InternalSelectCommand = new RelayCommand(this.Select);
        if (this.StartOpen)
        {
            this.IsOpen = true;
        }
    }

    public Brush SelectedForeground
    {
        get => (Brush)this.GetValue(SelectedForegroundProperty);
        set => this.SetValue(SelectedForegroundProperty, value);
    }

    public static readonly DependencyProperty SelectedForegroundProperty = DependencyProperty.Register(
        nameof(SelectedForeground),
        typeof(Brush),
        typeof(HamburgerMenuItem),
        new FrameworkPropertyMetadata(DefaultMouseOverProperty));

    public Brush SubtitleForeground
    {
        get => (Brush)this.GetValue(SubtitleForegroundProperty);
        set => this.SetValue(SubtitleForegroundProperty, value);
    }

    public static readonly DependencyProperty SubtitleForegroundProperty = DependencyProperty.Register(
        nameof(SubtitleForeground),
        typeof(Brush),
        typeof(HamburgerMenuItem),
        new FrameworkPropertyMetadata(DefaultMouseOverProperty));

    public Brush MouseOverBrush
    {
        get => (Brush)this.GetValue(MouseOverBrushProperty);
        set => this.SetValue(MouseOverBrushProperty, value);
    }

    public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(
        nameof(MouseOverBrush),
        typeof(Brush),
        typeof(HamburgerMenuItem),
        new FrameworkPropertyMetadata(DefaultMouseOverProperty));

    public Brush MouseDownBrush
    {
        get => (Brush)this.GetValue(MouseDownBrushProperty);
        set => this.SetValue(MouseDownBrushProperty, value);
    }

    public static readonly DependencyProperty MouseDownBrushProperty = DependencyProperty.Register(
        nameof(MouseDownBrush),
        typeof(Brush),
        typeof(HamburgerMenuItem));

    public Brush AccentBrush
    {
        get => (Brush)this.GetValue(AccentBrushProperty);
        set => this.SetValue(AccentBrushProperty, value);
    }

    public static readonly DependencyProperty AccentBrushProperty = DependencyProperty.Register(
        nameof(AccentBrush),
        typeof(Brush),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(new SolidColorBrush(Colors.DeepSkyBlue)));

    public DataTemplate IconTemplate
    {
        get => (DataTemplate)this.GetValue(IconTemplateProperty);
        set => this.SetValue(IconTemplateProperty, value);
    }

    public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register(
        nameof(IconTemplate),
        typeof(DataTemplate),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public DataTemplate ChildItemTemplate
    {
        get => (DataTemplate)this.GetValue(ChildItemTemplateProperty);
        set => this.SetValue(ChildItemTemplateProperty, value);
    }

    public static readonly DependencyProperty ChildItemTemplateProperty = DependencyProperty.Register(
        nameof(ChildItemTemplate),
        typeof(DataTemplate),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public DataTemplate ChildItemsTemplate
    {
        get => (DataTemplate)this.GetValue(ChildItemsTemplateProperty);
        set => this.SetValue(ChildItemsTemplateProperty, value);
    }

    public static readonly DependencyProperty ChildItemsTemplateProperty = DependencyProperty.Register(
        nameof(ChildItemsTemplate),
        typeof(DataTemplate),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public object Icon
    {
        get => this.GetValue(IconProperty);
        set => this.SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(object),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public DataTemplate TitleTemplate
    {
        get => (DataTemplate)this.GetValue(TitleTemplateProperty);
        set => this.SetValue(TitleTemplateProperty, value);
    }

    public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register(
        nameof(TitleTemplate),
        typeof(DataTemplate),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public string Title
    {
        get => (string)this.GetValue(TitleProperty);
        set => this.SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public DataTemplate SubTitleTemplate
    {
        get => (DataTemplate)this.GetValue(SubTitleTemplateProperty);
        set => this.SetValue(SubTitleTemplateProperty, value);
    }

    public static readonly DependencyProperty SubTitleTemplateProperty = DependencyProperty.Register(
        nameof(SubTitleTemplate),
        typeof(DataTemplate),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public string SubTitle
    {
        get => (string)this.GetValue(SubTitleProperty);
        set => this.SetValue(SubTitleProperty, value);
    }

    public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(
        nameof(SubTitle),
        typeof(string),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public bool IsOpen
    {
        get => (bool)this.GetValue(IsOpenProperty);
        set => this.SetValue(IsOpenProperty, value);
    }

    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
        nameof(IsOpen),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(false));

    public bool StartOpen
    {
        get => (bool)this.GetValue(StartOpenProperty);
        set => this.SetValue(StartOpenProperty, value);
    }

    public static readonly DependencyProperty StartOpenProperty = DependencyProperty.Register(
        nameof(StartOpen),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(false, OnSetStartOpen));

    public bool ShowOpenIndicator
    {
        get => (bool)this.GetValue(ShowOpenIndicatorProperty);
        set => this.SetValue(ShowOpenIndicatorProperty, value);
    }

    public static readonly DependencyProperty ShowOpenIndicatorProperty = DependencyProperty.Register(
        nameof(ShowOpenIndicator),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(true));

    public bool IsSelected
    {
        get => (bool)this.GetValue(IsSelectedProperty);
        set => this.SetValue(IsSelectedProperty, value);
    }

    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
        nameof(IsSelected),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(false));

    public bool IsContainingMenuOpen
    {
        get => (bool)this.GetValue(IsContainingMenuOpenProperty);
        set => this.SetValue(IsContainingMenuOpenProperty, value);
    }

    public static readonly DependencyProperty IsContainingMenuOpenProperty = DependencyProperty.Register(
        nameof(IsContainingMenuOpen),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(true));

    public bool ReserveIconSpace
    {
        get => (bool)this.GetValue(ReserveIconSpaceProperty);
        set => this.SetValue(ReserveIconSpaceProperty, value);
    }

    public static readonly DependencyProperty ReserveIconSpaceProperty = DependencyProperty.Register(
        nameof(ReserveIconSpace),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(false));

    public IEnumerable<object> ChildItems
    {
        get => (IEnumerable<object>)this.GetValue(ChildItemsProperty);
        set => this.SetValue(ChildItemsProperty, value);
    }

    public static readonly DependencyProperty ChildItemsProperty = DependencyProperty.Register(
        nameof(ChildItems),
        typeof(IEnumerable<object>),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(new ObservableCollection<object>()));

    public bool CanAddChild
    {
        get => (bool)this.GetValue(CanAddChildProperty);
        set => this.SetValue(CanAddChildProperty, value);
    }

    public static readonly DependencyProperty CanAddChildProperty = DependencyProperty.Register(
        nameof(CanAddChild),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(true));

    public bool CanDelete
    {
        get => (bool)this.GetValue(CanDeleteProperty);
        set => this.SetValue(CanDeleteProperty, value);
    }

    public static readonly DependencyProperty CanDeleteProperty = DependencyProperty.Register(
        nameof(CanDelete),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(true));

    public bool CanOpen
    {
        get => (bool)this.GetValue(CanOpenProperty);
        set => this.SetValue(CanOpenProperty, value);
    }

    public static readonly DependencyProperty CanOpenProperty = DependencyProperty.Register(
        nameof(CanOpen),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(true));

    public ICommand AddChildCommand
    {
        get => (ICommand)this.GetValue(AddChildCommandProperty);
        set => this.SetValue(AddChildCommandProperty, value);
    }

    public static readonly DependencyProperty AddChildCommandProperty = DependencyProperty.Register(
        nameof(AddChildCommand),
        typeof(ICommand),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public ICommand DeleteCommand
    {
        get => (ICommand)this.GetValue(DeleteCommandProperty);
        set => this.SetValue(DeleteCommandProperty, value);
    }

    public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
        nameof(DeleteCommand),
        typeof(ICommand),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public ICommand SelectCommand
    {
        get => (ICommand)this.GetValue(SelectCommandProperty);
        set => this.SetValue(SelectCommandProperty, value);
    }

    public static readonly DependencyProperty SelectCommandProperty = DependencyProperty.Register(
        nameof(SelectCommand),
        typeof(ICommand),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    internal ICommand InternalSelectCommand
    {
        get => (ICommand)this.GetValue(InternalSelectCommandProperty);
        set => this.SetValue(InternalSelectCommandProperty, value);
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
        if (this.ChildItems.Any() && this.CanOpen)
        {
            this.IsOpen = !this.IsOpen;
            return;
        }

        if (this.SelectCommand != null)
        {
            this.SelectCommand.Execute(this);
        }
    }
}
