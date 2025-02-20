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
    public static readonly DependencyProperty SelectedForegroundProperty = DependencyProperty.Register(
        nameof(SelectedForeground),
        typeof(Brush),
        typeof(HamburgerMenuItem));

    public static readonly DependencyProperty SubtitleForegroundProperty = DependencyProperty.Register(
        nameof(SubtitleForeground),
        typeof(Brush),
        typeof(HamburgerMenuItem));

    public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(
        nameof(MouseOverBrush),
        typeof(Brush),
        typeof(HamburgerMenuItem));

    public static readonly DependencyProperty MouseDownBrushProperty = DependencyProperty.Register(
        nameof(MouseDownBrush),
        typeof(Brush),
        typeof(HamburgerMenuItem));

    public static readonly DependencyProperty AccentBrushProperty = DependencyProperty.Register(
        nameof(AccentBrush),
        typeof(Brush),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(new SolidColorBrush(Colors.DeepSkyBlue)));

    public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register(
        nameof(IconTemplate),
        typeof(DataTemplate),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public static readonly DependencyProperty ChildItemTemplateProperty = DependencyProperty.Register(
        nameof(ChildItemTemplate),
        typeof(DataTemplate),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public static readonly DependencyProperty ChildItemsTemplateProperty = DependencyProperty.Register(
        nameof(ChildItemsTemplate),
        typeof(DataTemplate),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(object),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register(
        nameof(TitleTemplate),
        typeof(DataTemplate),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public static readonly DependencyProperty SubTitleTemplateProperty = DependencyProperty.Register(
        nameof(SubTitleTemplate),
        typeof(DataTemplate),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(
        nameof(SubTitle),
        typeof(string),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(false));

    public static readonly DependencyProperty StartExpandedProperty = DependencyProperty.Register(
        nameof(StartExpanded),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(false, OnSetStartOpen));

    public static readonly DependencyProperty ShowOpenIndicatorProperty = DependencyProperty.Register(
        nameof(ShowOpenIndicator),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(true));

    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
        nameof(IsSelected),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(false));

    public static readonly DependencyProperty IsContainingMenuOpenProperty = DependencyProperty.Register(
        nameof(IsContainingMenuOpen),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(true));

    public static readonly DependencyProperty ReserveIconSpaceProperty = DependencyProperty.Register(
        nameof(ReserveIconSpace),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(false));

    public static readonly DependencyProperty ChildItemsProperty = DependencyProperty.Register(
        nameof(ChildItems),
        typeof(IEnumerable<object>),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(new ObservableCollection<object>()));

    public static readonly DependencyProperty CanAddChildProperty = DependencyProperty.Register(
        nameof(CanAddChild),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(true));

    public static readonly DependencyProperty CanDeleteProperty = DependencyProperty.Register(
        nameof(CanDelete),
        typeof(bool),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(true));

    public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(
        nameof(SelectionMode),
        typeof(ViewModels.SelectionMode),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(ViewModels.SelectionMode.Automatic));

    public static readonly DependencyProperty AddChildCommandProperty = DependencyProperty.Register(
        nameof(AddChildCommand),
        typeof(ICommand),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
        nameof(DeleteCommand),
        typeof(ICommand),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public static readonly DependencyProperty SelectCommandProperty = DependencyProperty.Register(
        nameof(SelectCommand),
        typeof(ICommand),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    public static readonly DependencyProperty InternalSelectCommandProperty = DependencyProperty.Register(
        nameof(InternalSelectCommand),
        typeof(ICommand),
        typeof(HamburgerMenuItem),
        new PropertyMetadata(null));

    static HamburgerMenuItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HamburgerMenuItem), new FrameworkPropertyMetadata(typeof(HamburgerMenuItem)));
    }

    public HamburgerMenuItem()
    {
        this.InternalSelectCommand = new RelayCommand(this.Select);
        if (this.StartExpanded)
        {
            this.IsExpanded = true;
        }
    }

    public Brush SelectedForeground
    {
        get => (Brush)this.GetValue(SelectedForegroundProperty);
        set => this.SetValue(SelectedForegroundProperty, value);
    }

    public Brush SubtitleForeground
    {
        get => (Brush)this.GetValue(SubtitleForegroundProperty);
        set => this.SetValue(SubtitleForegroundProperty, value);
    }

    public Brush MouseOverBrush
    {
        get => (Brush)this.GetValue(MouseOverBrushProperty);
        set => this.SetValue(MouseOverBrushProperty, value);
    }

    public Brush MouseDownBrush
    {
        get => (Brush)this.GetValue(MouseDownBrushProperty);
        set => this.SetValue(MouseDownBrushProperty, value);
    }

    public Brush AccentBrush
    {
        get => (Brush)this.GetValue(AccentBrushProperty);
        set => this.SetValue(AccentBrushProperty, value);
    }

    public DataTemplate IconTemplate
    {
        get => (DataTemplate)this.GetValue(IconTemplateProperty);
        set => this.SetValue(IconTemplateProperty, value);
    }

    public DataTemplate ChildItemTemplate
    {
        get => (DataTemplate)this.GetValue(ChildItemTemplateProperty);
        set => this.SetValue(ChildItemTemplateProperty, value);
    }

    public DataTemplate ChildItemsTemplate
    {
        get => (DataTemplate)this.GetValue(ChildItemsTemplateProperty);
        set => this.SetValue(ChildItemsTemplateProperty, value);
    }

    public object Icon
    {
        get => this.GetValue(IconProperty);
        set => this.SetValue(IconProperty, value);
    }

    public DataTemplate TitleTemplate
    {
        get => (DataTemplate)this.GetValue(TitleTemplateProperty);
        set => this.SetValue(TitleTemplateProperty, value);
    }

    public string Title
    {
        get => (string)this.GetValue(TitleProperty);
        set => this.SetValue(TitleProperty, value);
    }

    public DataTemplate SubTitleTemplate
    {
        get => (DataTemplate)this.GetValue(SubTitleTemplateProperty);
        set => this.SetValue(SubTitleTemplateProperty, value);
    }

    public string SubTitle
    {
        get => (string)this.GetValue(SubTitleProperty);
        set => this.SetValue(SubTitleProperty, value);
    }

    public bool IsExpanded
    {
        get => (bool)this.GetValue(IsExpandedProperty);
        set => this.SetValue(IsExpandedProperty, value);
    }

    public bool StartExpanded
    {
        get => (bool)this.GetValue(StartExpandedProperty);
        set => this.SetValue(StartExpandedProperty, value);
    }

    public bool ShowOpenIndicator
    {
        get => (bool)this.GetValue(ShowOpenIndicatorProperty);
        set => this.SetValue(ShowOpenIndicatorProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)this.GetValue(IsSelectedProperty);
        set => this.SetValue(IsSelectedProperty, value);
    }

    public bool IsContainingMenuOpen
    {
        get => (bool)this.GetValue(IsContainingMenuOpenProperty);
        set => this.SetValue(IsContainingMenuOpenProperty, value);
    }

    public bool ReserveIconSpace
    {
        get => (bool)this.GetValue(ReserveIconSpaceProperty);
        set => this.SetValue(ReserveIconSpaceProperty, value);
    }

    public IEnumerable<object> ChildItems
    {
        get => (IEnumerable<object>)this.GetValue(ChildItemsProperty);
        set => this.SetValue(ChildItemsProperty, value);
    }

    public bool CanAddChild
    {
        get => (bool)this.GetValue(CanAddChildProperty);
        set => this.SetValue(CanAddChildProperty, value);
    }

    public bool CanDelete
    {
        get => (bool)this.GetValue(CanDeleteProperty);
        set => this.SetValue(CanDeleteProperty, value);
    }

    public ViewModels.SelectionMode SelectionMode
    {
        get => (ViewModels.SelectionMode)this.GetValue(SelectionModeProperty);
        set => this.SetValue(SelectionModeProperty, value);
    }

    public ICommand AddChildCommand
    {
        get => (ICommand)this.GetValue(AddChildCommandProperty);
        set => this.SetValue(AddChildCommandProperty, value);
    }

    public ICommand DeleteCommand
    {
        get => (ICommand)this.GetValue(DeleteCommandProperty);
        set => this.SetValue(DeleteCommandProperty, value);
    }

    public ICommand SelectCommand
    {
        get => (ICommand)this.GetValue(SelectCommandProperty);
        set => this.SetValue(SelectCommandProperty, value);
    }

    internal ICommand InternalSelectCommand
    {
        get => (ICommand)this.GetValue(InternalSelectCommandProperty);
        set => this.SetValue(InternalSelectCommandProperty, value);
    }

    private static void OnSetStartOpen(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is HamburgerMenuItem this_ && this_.StartExpanded && !this_.IsExpanded)
        {
            this_.IsExpanded = true;
        }
    }

    private void Select()
    {
        switch (this.SelectionMode)
        {
            case ViewModels.SelectionMode.Automatic:
                if (this.ChildItems.Any())
                {
                    this.IsExpanded = !this.IsExpanded;
                    break;
                }

                if (this.SelectCommand != null)
                {
                    this.SelectCommand.Execute(this);
                }

                break;

            case ViewModels.SelectionMode.Select:
                if (this.SelectCommand != null)
                {
                    this.SelectCommand.Execute(this);
                }

                break;

            case ViewModels.SelectionMode.Expand:
                this.IsExpanded = !this.IsExpanded;
                break;
        }
    }
}
