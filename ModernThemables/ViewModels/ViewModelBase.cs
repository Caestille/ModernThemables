using CoreUtilities.HelperClasses;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows.Input;
using System;
using System.Linq;
using ModernThemables.Messages;

namespace ModernThemables.ViewModels
{
	public class ViewModelBase : ViewModelBase<GenericViewModelBase>
	{
		public ViewModelBase(string name) : base(name) { }
	}

    public class ViewModelBase<TChild> : GenericViewModelBase where TChild : GenericViewModelBase
    {
        public ICommand AddChildCommand => new RelayCommand(() => AddChild());
        public ICommand RequestDeleteCommand => new RelayCommand(OnDeleteRequested);

        private readonly Func<TChild>? createChildFunc;

        private RangeObservableCollection<TChild> childViewModels = new();
        public RangeObservableCollection<TChild> ChildViewModels
        {
            get => childViewModels;
            set => SetProperty(ref childViewModels, value);
        }

        public virtual bool SupportsAddingChildren => createChildFunc != null;

        public ViewModelBase(string name, Func<TChild>? createChild = null)
            : base(name)
        {
            createChildFunc = createChild;
            BindMessages();
        }

        protected virtual void BindMessages()
        {
            Messenger.Register<ViewModelRequestShowMessage>(this, (sender, message) =>
            {
                if (message.ViewModel == this)
                    OnViewModelRequestShow(message);
                else if (IsSelected)
                    IsSelected = false;
            });

            Messenger.Register<ViewModelRequestDeleteMessage>(this, (sender, message) =>
            {
                OnViewModelRequestDelete(message);
            });
        }

        protected virtual void OnViewModelRequestShow(ViewModelRequestShowMessage message)
        {
            if (IsSelected && message.ViewModel != this)
            {
                IsSelected = false;
            }
        }

		protected virtual void OnDeleteRequested()
		{
			Messenger.Send(new ViewModelRequestDeleteMessage(this));
		}

		protected virtual void OnViewModelRequestDelete(ViewModelRequestDeleteMessage message)
		{
			if (message.ViewModel is TChild child && ChildViewModels.Contains(child))
			{
				ChildViewModels.Remove(child);

				if (IsShowingChildren && !ChildViewModels.Any())
				{
					IsShowingChildren = false;
				}

				OnPropertyChanged(nameof(ChildViewModels));
				OnChildrenChanged();
			}
		}

		protected override void Select()
		{
			if (ChildViewModels.Count != 0)
			{
				IsShowingChildren = !IsShowingChildren;
			}
			else if (!SupportsAddingChildren)
			{
				Messenger.Send(new ViewModelRequestShowMessage(this));
			}
		}

		public virtual void AddChild(TChild? viewModelToAdd = null, string name = "")
		{
			var viewModel = viewModelToAdd ?? (createChildFunc != null ? createChildFunc() : null);
			if (viewModel is null)
			{
				return;
			}

			if (name != string.Empty)
			{
				viewModel.Name = name;
			}

			ChildViewModels.Add(viewModel);

			if (SupportsAddingChildren)
			{
				IsShowingChildren = true;
			}

			OnPropertyChanged(nameof(ChildViewModels));
			OnChildrenChanged();
		}

		public override void GetChildren(ref RangeObservableCollection<GenericViewModelBase> result, bool recurse)
		{
			result.AddRange(ChildViewModels);

			if (!recurse)
				return;

			foreach (var childVm in ChildViewModels)
			{
				childVm.GetChildren(ref result, true);
			}
		}

		protected virtual void OnChildrenChanged() { }
	}
}
