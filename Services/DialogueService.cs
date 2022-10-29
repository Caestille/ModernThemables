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
	/// <summary>
	/// A service for initialising and managing various dialogues.
	/// </summary>
	public class DialogueService : IDialogueService
	{
		private Dictionary<Type, Type> registeredViews = new();

		/// <summary>
		/// Registers a given view <see cref="Type"/> to a given viewmodel <see cref="Type"/>. When
		/// <see cref="OpenCustomDialogue(object?, Size?)"/> is called with a given <see cref="object"/> (usually a
		/// data context), if the type of the given object is registered, a dialogue will be opened hosting the
		/// registered view.
		/// </summary>
		/// <param name="viewType">The type of view to register to the viewmodel.</param>
		/// <param name="vmType">The type of viewmodel to open the view for.</param>
		public void RegisterViewForViewModel(Type viewType, Type vmType)
		{
			registeredViews[vmType] = viewType;
		}

		/// <summary>
		/// Opens a dialogue for a given <see cref="object"/> if the object type matches a registered view and
		/// viewmodel <see cref="Type"/> using <see cref="RegisterViewForViewModel(Type, Type)"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="object"/> data context to open the view for.</param>
		/// <param name="dialogueSize">An overriding dialogue size in case an auto sized dialogue is not desired.
		/// </param>
		/// <returns>An awaitable <see cref="Task"/>.</returns>
		public async Task OpenCustomDialogue(object? dataContext=null, Size? dialogueSize = null)
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

		/// <summary>
		/// Opens the OS default <see cref="OpenFileDialog"/>.
		/// </summary>
		/// <returns>The path result of the <see cref="OpenFileDialog"/>.</returns>
		public string OpenFileDialogue()
		{
			var dialogue = new OpenFileDialog();

			if (dialogue.ShowDialog() == true)
			{
				return dialogue.FileName;
			}

			return string.Empty;
		}

		/// <summary>
		/// Shows a message box to the user.
		/// </summary>
		/// <param name="title">The message box title.</param>
		/// <param name="message">The message in the message box.</param>
		/// <param name="button">The message acknowledgement types to show on the message box.</param>
		public void ShowMessageBox(string title, string message, MessageBoxButton button)
		{
			
		}
	}
}
