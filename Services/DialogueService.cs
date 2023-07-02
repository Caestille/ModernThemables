using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ModernThemables.Controls;
using CoreUtilities.Interfaces.Dialogues;
using System.Windows.Controls;

namespace ModernThemables.Services
{
	/// <summary>
	/// A service for initialising and managing various dialogues.
	/// </summary>
	public class DialogueService : IDialogueService
	{
		private Dictionary<Type, Type> registeredViews = new();

		/// <inheritdoc />
		public void RegisterViewForViewModel(Type viewType, Type vmType)
		{
			registeredViews[vmType] = viewType;
		}

		/// <inheritdoc />
		public async Task ShowCustomDialogue(object? dataContext=null, Size? dialogueSize = null)
		{
			var window = new ThemableWindow2();
			window.UseLayoutRounding = true;
			window.SnapsToDevicePixels = true;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.Owner = Application.Current.MainWindow;
			window.FontFamily = window.Owner.FontFamily;
			window.FontSize = window.Owner.FontSize;
			window.Icon = window.Owner.Icon;
			window.HorizontalContentAlignment = HorizontalAlignment.Center;
			window.VerticalContentAlignment = VerticalAlignment.Center;
			window.Resources.Add(new DataTemplateKey(dataContext.GetType()), new DataTemplate()
			{
				DataType = dataContext.GetType(),
				VisualTree = new FrameworkElementFactory(registeredViews[dataContext.GetType()]),
			});
			window.Content = new ContentControl() { Content = (ObservableObject)dataContext };
			if (dialogueSize == null)
			{
				window.SizeToContent = SizeToContent.WidthAndHeight;
			}
			else
			{
				window.Width = dialogueSize.Value.Width;
				window.Height = dialogueSize.Value.Height;
			}
			window.WindowStyle = WindowStyle.ToolWindow;
			RenderOptions.SetBitmapScalingMode(window, BitmapScalingMode.HighQuality);
			RenderOptions.SetClearTypeHint(window, ClearTypeHint.Enabled);

			window.ShowDialog();
		}

		/// <inheritdoc />
		public async Task ShowBorderlessCustomDialogue(object? dataContext = null, Size? dialogueSize = null)
		{
			var window = new Window();
			window.WindowStyle = WindowStyle.None;
			window.AllowsTransparency = true;
			window.Background = new SolidColorBrush(Colors.Transparent);
			window.UseLayoutRounding = true;
			window.SnapsToDevicePixels = true;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.Owner = Application.Current.MainWindow;
			window.FontFamily = window.Owner.FontFamily;
			window.FontSize = window.Owner.FontSize;
			window.Icon = window.Owner.Icon;
			window.HorizontalContentAlignment = HorizontalAlignment.Center;
			window.VerticalContentAlignment = VerticalAlignment.Center;
			window.Resources.Add(new DataTemplateKey(dataContext.GetType()), new DataTemplate()
			{
				DataType = dataContext.GetType(),
				VisualTree = new FrameworkElementFactory(registeredViews[dataContext.GetType()]),
			});
			window.Content = new ContentControl() { Content = (ObservableObject)dataContext };
			if (dialogueSize == null)
			{
				window.SizeToContent = SizeToContent.WidthAndHeight;
			}
			else
			{
				window.Width = dialogueSize.Value.Width;
				window.Height = dialogueSize.Value.Height;
			}
			RenderOptions.SetBitmapScalingMode(window, BitmapScalingMode.HighQuality);
			RenderOptions.SetClearTypeHint(window, ClearTypeHint.Enabled);

			window.ShowDialog();
		}

		/// <inheritdoc />
		public string ShowOpenFileDialogue()
		{
			var dialogue = new OpenFileDialog();

			if (dialogue.ShowDialog() == true)
			{
				return dialogue.FileName;
			}

			return string.Empty;
		}

		/// <inheritdoc />
		public Color ShowColourPickerDialogue(Color inputColour, Action<Color>? colourChangedCallback = null)
		{
			var dialogue = new ColourPickerDialogue(inputColour, colourChangedCallback);

			dialogue.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			dialogue.Owner = Application.Current.MainWindow;
			dialogue.WindowStyle = WindowStyle.ToolWindow;

			if (dialogue.ShowDialog() == true)
			{
				return dialogue.Colour;
			}

			return inputColour;
		}

		/// <inheritdoc />
		public void ShowMessageBox(string title, string message, MessageBoxButton button)
		{
			
		}
	}
}
