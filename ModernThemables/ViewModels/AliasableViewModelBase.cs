namespace ModernThemables.ViewModels;

using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

public class AliasableViewModelBase : AliasableViewModelBase<GenericViewModelBase>
{
    public AliasableViewModelBase(string name, string? alias, Func<GenericViewModelBase>? createChild = null)
        : base(name, alias, createChild) { }
}

public class AliasableViewModelBase<TChild> : ViewModelBase<TChild>
    where TChild : GenericViewModelBase
{
    private string? previousAlias;
    private bool isEditingAlias;
    private string? alias;

    public AliasableViewModelBase(
        string name, string? alias, Func<TChild>? createChild = null)
        : base(name, createChild)
    {
        this.Alias = alias;
    }

    public ICommand EditAliasCommand => new RelayCommand(this.EditAlias);

    public ICommand AliasEditorKeyDownCommand => new RelayCommand<object>(this.NameEditorKeyDown);

    public bool IsEditingAlias
    {
        get => this.isEditingAlias;
        set => this.SetProperty(ref this.isEditingAlias, value);
    }

    public string? Alias
    {
        get => this.alias;
        set => this.SetProperty(ref this.alias, value);
    }

    public override string Name => string.IsNullOrWhiteSpace(this.Alias) ? base.Name : this.Alias;

    public string OriginalName => base.Name;

    protected virtual void OnCommitAliasUpdate() => this.OnPropertyChanged(nameof(this.Name));

    private void EditAlias()
    {
        this.IsEditingAlias = !this.IsEditingAlias;
        this.previousAlias = this.Alias;
        if (this.IsEditingAlias)
        {
            this.Alias = this.Name;
        }
    }

    private void NameEditorKeyDown(object? args)
    {
        if (args != null && args is KeyEventArgs e && (e.Key == Key.Enter || e.Key == Key.Escape))
        {
            if (e.Key == Key.Escape)
            {
                this.Alias = this.previousAlias;
            }
            else
            {
                this.OnCommitAliasUpdate();
            }

            this.IsEditingAlias = false;
        }
    }
}
