namespace ModernThemables.ViewModels;

#pragma warning disable SA1402 // File may only contain a single type

using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CoreUtilities.Helpers.WPF;
using ModernThemables.Messages;

public abstract class ViewModelBase : ViewModelBase<GenericViewModelBase>
{
    public ViewModelBase(string name)
        : base(name) { }
}

public abstract class ViewModelBase<TChild> : GenericViewModelBase
    where TChild : GenericViewModelBase
{
    private readonly Func<TChild>? createChildFunc;
    private RangeObservableCollection<TChild> childViewModels = new();

    public ViewModelBase(string name, Func<TChild>? createChild = null)
        : base(name)
    {
        this.createChildFunc = createChild;
        this.BindMessages();
    }

    public ICommand? AddChildCommand => this.createChildFunc != null ? new RelayCommand(() => this.AddChild()) : null;

    public RangeObservableCollection<TChild> ChildViewModels
    {
        get => this.childViewModels;
        set => this.SetProperty(ref this.childViewModels, value);
    }

    public virtual void AddChild(TChild? viewModelToAdd = null, string name = "", int? index = null)
    {
        var viewModel = viewModelToAdd ?? (this.createChildFunc != null ? this.createChildFunc() : null);
        if (viewModel is null)
        {
            return;
        }

        if (name != string.Empty)
        {
            viewModel.Name = name;
        }

        if (index == null)
        {
            this.ChildViewModels.Add(viewModel);
        }
        else
        {
            this.ChildViewModels.Insert(index.Value, viewModel);
        }

        this.OnPropertyChanged(nameof(this.ChildViewModels));
        this.OnChildrenChanged();
    }

    public override List<object> GetChildren(bool recurse = false)
    {
        var result = new List<object>();
        result.AddRange(this.ChildViewModels);

        if (!recurse)
        {
            return result;
        }

        foreach (var childVm in this.ChildViewModels)
        {
            result.AddRange(childVm.GetChildren(true));
        }

        return result;
    }

    protected virtual void BindMessages()
    {
        this.Messenger.Register<ViewModelRequestShowMessage>(this, (sender, message) =>
        {
            //if (message.ViewModel == this)
            //{
                this.OnRequestShowReceived(message);
            //}
            //else if (this.IsSelected)
            //{
            //    this.IsSelected = false;
            //}
        });

        this.Messenger.Register<ViewModelRequestDeleteMessage>(this, (sender, message) =>
        {
            this.OnRequestDeleteReceived(message);
        });
    }

    protected virtual void OnRequestShowReceived(ViewModelRequestShowMessage message)
    {
        if (this.IsSelected && message.ViewModel != this)
        {
            this.IsSelected = false;
        }
    }

    protected virtual void OnRequestDeleteReceived(ViewModelRequestDeleteMessage message)
    {
        if (message.ViewModel is TChild child && this.ChildViewModels.Contains(child))
        {
            child.OnDelete();
            this.ChildViewModels.Remove(child);

            this.OnPropertyChanged(nameof(this.ChildViewModels));
            this.OnChildrenChanged();
        }
    }

    protected virtual void OnChildrenChanged() { }
}
