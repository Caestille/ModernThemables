using CoreUtilities.HelperClasses;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Diagnostics.CodeAnalysis;

namespace ModernThemables.ViewModels
{
	public class GenericViewModelBase : ObservableRecipient
	{
		private readonly IEnumerable<Action<Color>> notifyColourUpdates = new List<Action<Color>>();
		public ICommand SelectCommand => new RelayCommand(Select);

		public static string? WorkingDirectory { protected get; set; }

		private string name = string.Empty;
		public virtual string Name
		{
			get => name;
			set => SetProperty(ref name, value.TrimStart());
		}

		private Color colour = Colors.Red;
		public Color Colour
		{
			get => colour;
			set
			{
				SetProperty(ref colour, value);
				OnCommitColourUpdate();
			}
		}

		private bool isSelected;
		public bool IsSelected
		{
			get => isSelected;
			set => SetProperty(ref isSelected, value);
		}

		private bool isDisplayed;
		public bool IsDisplayed
		{
			get => isDisplayed;
			set => SetProperty(ref isDisplayed, value);
		}

		private bool isMenuItemExpanded;
		public bool IsMenuItemExpanded
		{
			get => isMenuItemExpanded;
			set => SetProperty(ref isMenuItemExpanded, value);
		}

		private bool supportsDeleting;
		public bool SupportsDeleting
		{
			get => supportsDeleting;
			set => SetProperty(ref supportsDeleting, value);
		}

		private bool canDropDown = true;
		public bool CanDropDown
		{
			get => canDropDown;
			set => SetProperty(ref canDropDown, value);
        }

        private bool isShowingChildren;
        public bool IsShowingChildren
        {
            get => isShowingChildren;
            set => SetProperty(ref isShowingChildren, value);
        }

        protected IMessenger BaseMessenger => Messenger;

		public GenericViewModelBase(string name)
		{
			Name = name;
			Application.Current.Dispatcher.ShutdownStarted += OnShutdownStart;
		}

		public void RegisterColourUpdateNotification(Action<Color> toInvoke)
		{
			notifyColourUpdates.Append(toInvoke);
		}

		protected virtual void Select() { }

		public virtual void GetChildren(ref RangeObservableCollection<GenericViewModelBase> result, bool recurse) { }

		protected virtual void OnCommitColourUpdate()
		{
			if (notifyColourUpdates.Any())
			{
				foreach (var action in notifyColourUpdates)
				{
					action(Colour);
				}
			}
		}

		protected virtual void OnShutdownStart(object? sender, EventArgs e) { }
	}
}
