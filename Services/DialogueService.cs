using CoreUtilities.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using Win10Themables.Controls;

namespace Win10Themables.Services
{
	public class DialogueService : IDialogueService
	{
		public Dictionary<Type, Type> registeredViews = new();

		public void RegisterViewForViewModel(Type viewType, Type vmType)
		{
			registeredViews[vmType] = viewType;
		}

		public async Task OpenCustomDialogue(object? dataContext=null)
		{
			Window window = new Window();
			window.AllowsTransparency = false;
			window.FontFamily = (FontFamily)Application.Current.Resources["OpenSans"];
			window.FontSize = 13;
			window.UseLayoutRounding = true;
			window.SnapsToDevicePixels = true;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.Owner = Application.Current.MainWindow;
			window.Resources.Add(new DataTemplateKey(dataContext.GetType()), new DataTemplate()
			{
				DataType = dataContext.GetType(),
				VisualTree = new FrameworkElementFactory(registeredViews[dataContext.GetType()]),
			});
			window.Width = 100;
			window.Height = 100;
			window.Content = new MainWindowControl() { VisibleViewModel = (ObservableObject)dataContext, IsToolWindow = true };
			WindowChrome.SetWindowChrome(window, new WindowChrome() { ResizeBorderThickness = new Thickness(5), CaptionHeight = 24 });
			window.WindowStyle = WindowStyle.ToolWindow;
			RenderOptions.SetBitmapScalingMode(window, BitmapScalingMode.HighQuality);
			RenderOptions.SetClearTypeHint(window, ClearTypeHint.Enabled);

			window.ShowDialog();
		}

		public void OpenFileDialogue()
		{

		}

		public void ShowMessageBox(string title, string message, MessageBoxButton button)
		{
			
		}
	}
}
