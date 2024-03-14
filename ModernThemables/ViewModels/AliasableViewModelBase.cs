using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ModernThemables.ViewModels
{
	public class AliasableViewModelBase : AliasableViewModelBase<GenericViewModelBase>
	{
		public AliasableViewModelBase(string name, string alias, Func<GenericViewModelBase>? createChild = null) : base(name, alias, createChild) { }
	}

	public class AliasableViewModelBase<TChild> : ViewModelBase<TChild> where TChild : GenericViewModelBase
	{
		private string? previousAlias;

		public ICommand EditAliasCommand => new RelayCommand(EditAlias);
		public ICommand AliasEditorKeyDownCommand => new RelayCommand<object>(NameEditorKeyDown);

		private bool isEditingAlias;
		public bool IsEditingAlias
		{
			get => isEditingAlias;
			set => SetProperty(ref isEditingAlias, value);
		}

		private string? alias;
		public string? Alias
		{
			get => alias;
			set => SetProperty(ref alias, value);
		}

		public override string Name => string.IsNullOrWhiteSpace(Alias) ? base.Name : Alias;

		public string OriginalName => base.Name;

		public AliasableViewModelBase(
			string name, string alias, Func<TChild>? createChild = null)
			: base(name, createChild)
		{
			Alias = alias;
		}

		protected virtual void OnCommitAliasUpdate()
		{
			OnPropertyChanged(nameof(Name));
		}

		private void EditAlias()
		{
			IsEditingAlias = !IsEditingAlias;
			previousAlias = Alias;
			if (IsEditingAlias) Alias = Name;
		}

		private void NameEditorKeyDown(object? args)
		{
			if (args != null && args is KeyEventArgs e && (e.Key == Key.Enter || e.Key == Key.Escape))
			{
				if (e.Key == Key.Escape)
				{
					Alias = previousAlias;
				}
				else
				{
					OnCommitAliasUpdate();
				}

				IsEditingAlias = false;
			}
		}
	}
}
