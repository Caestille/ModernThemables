using CoreUtilities.HelperClasses;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows.Input;
using System;
using ModernThemables.Messages;
using System.Collections.Generic;

namespace ModernThemables.ViewModels
{
    public class ViewModelBase : ViewModelBase<GenericViewModelBase>
	{
		public ViewModelBase(string name) : base(name) { }
	}

    public class ViewModelBase<TChild> : GenericViewModelBase where TChild : GenericViewModelBase
    {
        public ICommand AddChildCommand => new RelayCommand(() => AddChild());

        private readonly Func<TChild>? createChildFunc;

        private RangeObservableCollection<TChild> childViewModels = new();
        public RangeObservableCollection<TChild> ChildViewModels
        {
            get => childViewModels;
            set => SetProperty(ref childViewModels, value);
        }

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
                    OnRequestShowReceived(message);
                else if (IsSelected)
                    IsSelected = false;
            });

            Messenger.Register<ViewModelRequestDeleteMessage>(this, (sender, message) =>
            {
                OnRequestDeleteReceived(message);
            });
        }

        protected virtual void OnRequestShowReceived(ViewModelRequestShowMessage message)
        {
            if (IsSelected && message.ViewModel != this)
            {
                IsSelected = false;
            }
        }

		protected virtual void OnRequestDeleteReceived(ViewModelRequestDeleteMessage message)
		{
			if (message.ViewModel is TChild child && ChildViewModels.Contains(child))
			{
                child.OnDelete();
				ChildViewModels.Remove(child);

				OnPropertyChanged(nameof(ChildViewModels));
				OnChildrenChanged();
			}
		}

		public virtual void AddChild(TChild? viewModelToAdd = null, string name = "", int? index = null)
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

            if (index == null)
            {
                ChildViewModels.Add(viewModel);
            }
            else
            {
                ChildViewModels.Insert(index.Value, viewModel);
            }

            OnPropertyChanged(nameof(ChildViewModels));
			OnChildrenChanged();
		}

		public override List<object> GetChildren(bool recurse = false)
		{
            var result = new List<object>();
			result.AddRange(ChildViewModels);

            if (!recurse)
                return result;


            foreach (var childVm in ChildViewModels)
			{
                result.AddRange(childVm.GetChildren(true));
			}

            return result;
		}

		protected virtual void OnChildrenChanged() { }
	}
}
