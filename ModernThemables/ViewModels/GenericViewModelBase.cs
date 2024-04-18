using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ModernThemables.ViewModels
{
    public interface IMenuItem
    {
        string Name { get; }
        List<object> GetChildren(bool recurse = false);
    }

    public abstract class GenericViewModelBase : ObservableRecipient, IMenuItem
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

        private bool isExpanded;
        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        private bool isDisplayed;
		public bool IsDisplayed
		{
			get => isDisplayed;
			set => SetProperty(ref isDisplayed, value);
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

        public abstract List<object> GetChildren(bool recurse = false);

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
