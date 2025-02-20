namespace ModernThemables.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ModernThemables.Messages;

public abstract class GenericViewModelBase : ObservableRecipient, IHamburgerMenuItem
{
    private readonly IEnumerable<Action<Color>> notifyColourUpdates = new List<Action<Color>>();

    private bool isDisplayed;
    private bool isExpanded;
    private bool isSelected;
    private Color colour = Colors.Red;
    private string name = string.Empty;

    public GenericViewModelBase(string name)
    {
        this.Name = name;
        Application.Current.Dispatcher.ShutdownStarted += this.OnShutdownStart;
    }

    public static string? WorkingDirectory { protected get; set; }

    public ICommand SelectCommand => new RelayCommand(() => this.Select(this));

    public ICommand DeleteCommand => new RelayCommand(this.Delete);

    public abstract NavigationProperties NavigationProperties { get; }

    public virtual string Name
    {
        get => this.name;
        set => this.SetProperty(ref this.name, value.TrimStart());
    }

    public Color Colour
    {
        get => this.colour;
        set
        {
            this.SetProperty(ref this.colour, value);
            this.OnCommitColourUpdate();
        }
    }

    public bool IsSelected
    {
        get => this.isSelected;
        set => this.SetProperty(ref this.isSelected, value);
    }

    public bool IsExpanded
    {
        get => this.isExpanded;
        set => this.SetProperty(ref this.isExpanded, value);
    }

    public bool IsDisplayed
    {
        get => this.isDisplayed;
        set => this.SetProperty(ref this.isDisplayed, value);
    }

    protected IMessenger BaseMessenger => this.Messenger;

    public void RegisterColourUpdateNotification(Action<Color> toInvoke)
        => this.notifyColourUpdates.Append(toInvoke);

    public virtual void Select(GenericViewModelBase? sender = null)
        => this.Messenger.Send(new ViewModelRequestShowMessage(this, sender ?? this));

    public virtual void Delete()
        => this.Messenger.Send(new ViewModelRequestDeleteMessage(this));

    public virtual void OnDelete() { }

    public abstract List<object> GetChildren(bool recurse = false);

    protected virtual void OnCommitColourUpdate()
    {
        if (this.notifyColourUpdates.Any())
        {
            foreach (var action in this.notifyColourUpdates)
            {
                action(this.Colour);
            }
        }
    }

    protected virtual void OnShutdownStart(object? sender, EventArgs e) { }
}
