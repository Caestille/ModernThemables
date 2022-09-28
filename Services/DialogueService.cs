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

namespace ModernThemables.Services
{
    public class DialogueService : IDialogueService
	{
		public Dictionary<Type, Type> registeredViews = new();

		public void RegisterViewForViewModel(Type viewType, Type vmType)
		{
			registeredViews[vmType] = viewType;
		}

		public async Task OpenCustomDialogue(object? dataContext=null, Size? dialogueSize = null)
		{
			Window window = new Window();
			window.AllowsTransparency = false;
			window.FontFamily = (FontFamily)Application.Current.Resources["OpenSans"];
			window.FontSize = 13;
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

		public string OpenFileDialogue()
		{
			var dialogue = new OpenFileDialog();

			if (dialogue.ShowDialog() == true)
			{
				return dialogue.FileName;
			}

			return string.Empty;
		}

		public void ShowMessageBox(string title, string message, MessageBoxButton button)
		{
			
		}
	}
}
