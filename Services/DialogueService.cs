using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
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
			Window window = new Window();
			window.AllowsTransparency = false;
			window.FontSize = 13;
			window.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Inter");
			window.UseLayoutRounding = true;
			window.SnapsToDevicePixels = true;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.Owner = Application.Current.MainWindow;
			window.HorizontalContentAlignment = HorizontalAlignment.Center;
			window.VerticalContentAlignment = VerticalAlignment.Center;
			window.Resources.Add(new DataTemplateKey(dataContext.GetType()), new DataTemplate()
			{
				DataType = dataContext.GetType(),
				VisualTree = new FrameworkElementFactory(registeredViews[dataContext.GetType()]),
			});
			window.Content = new MainWindowControl() { VisibleViewModel = (ObservableObject)dataContext, IsToolWindow = true };
			if (dialogueSize == null)
			{
				window.SizeToContent = SizeToContent.WidthAndHeight;
			}
			else
			{
				window.Width = dialogueSize.Value.Width;
				window.Height = dialogueSize.Value.Height;
			}
			WindowChrome.SetWindowChrome(window, new WindowChrome() { ResizeBorderThickness = new Thickness(5), CaptionHeight = 24 });
			window.WindowStyle = WindowStyle.ToolWindow;
			RenderOptions.SetBitmapScalingMode(window, BitmapScalingMode.HighQuality);
			RenderOptions.SetClearTypeHint(window, ClearTypeHint.Enabled);

			window.ShowDialog();
		}

		/// <inheritdoc />
		public async Task ShowBorderlessCustomDialogue(object? dataContext = null, Size? dialogueSize = null)
		{
			Window window = new Window();
			window.WindowStyle = WindowStyle.None;
			window.AllowsTransparency = true;
			window.Background = new SolidColorBrush(Colors.Transparent);
			window.FontSize = 13;
			window.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Inter");
			window.UseLayoutRounding = true;
			window.SnapsToDevicePixels = true;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.Owner = Application.Current.MainWindow;
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
			//WindowChrome.SetWindowChrome(window, new WindowChrome() { ResizeBorderThickness = new Thickness(5), CaptionHeight = 24 });
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
		public Color ShowColourPickerDialogue(Color inputColour)
		{
			var dialogue = new ColourPickerDialogue(inputColour);
			dialogue.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			dialogue.Owner = Application.Current.MainWindow;

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
