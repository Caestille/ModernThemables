namespace ModernThemables.ViewModels
{
    using CoreUtilities.HelperClasses;
    using CommunityToolkit.Mvvm.Input;
    using CommunityToolkit.Mvvm.Messaging;
    using System.Windows.Input;
    using System;
    using ModernThemables.Messages;
    using System.Collections.Generic;

    public class ViewModelBase : ViewModelBase<GenericViewModelBase>
	{
		public ViewModelBase(string name) : base(name) { }
	}

    public class ViewModelBase<TChild> : GenericViewModelBase where TChild : GenericViewModelBase
    {
        public ICommand AddChildCommand => new RelayCommand(() => this.AddChild());

        private readonly Func<TChild>? createChildFunc;

        private RangeObservableCollection<TChild> childViewModels = new();
        public RangeObservableCollection<TChild> ChildViewModels
        {
            get => this.childViewModels;
            set => this.SetProperty(ref this.childViewModels, value);
        }

        public ViewModelBase(string name, Func<TChild>? createChild = null)
            : base(name)
        {
            this.createChildFunc = createChild;
            this.BindMessages();
        }

        protected virtual void BindMessages()
        {
            this.Messenger.Register<ViewModelRequestShowMessage>(this, (sender, message) =>
            {
                if (message.ViewModel == this)
                    this.OnRequestShowReceived(message);
                else if (this.IsSelected)
                    this.IsSelected = false;
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

		public virtual void AddChild(TChild? viewModelToAdd = null, string name = string.Empty, int? index = null)
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
                return result;


            foreach (var childVm in this.ChildViewModels)
			{
                result.AddRange(childVm.GetChildren(true));
			}

            return result;
		}

		protected virtual void OnChildrenChanged() { }
	}
}
