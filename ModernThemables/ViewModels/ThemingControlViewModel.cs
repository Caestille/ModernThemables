using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CoreUtilities.Interfaces.RegistryInteraction;
using CoreUtilities.Services.RegistryInteraction;
using System.Timers;
using CoreUtilities.HelperClasses.Extensions;
using CoreUtilities.Interfaces.Dialogues;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;
//using ModernThemables.Services;

namespace ModernThemables.ViewModels
{
	/// <summary>
	/// A view model for a theming control to interact with the theme status of an application with.
	/// </summary>
	public partial class ThemingControlViewModel : ObservableObject
	{
		private readonly IDialogueService dialogueService;

        public event EventHandler<bool>? TransparentHeaderChanged;
        public event EventHandler<bool>? IsDarkChanged;
        public event EventHandler<bool>? SyncWithOsChanged;

        private Color themeColour;
        private bool isSyncingWithOs;
        private bool isDarkMode;
        private bool isTransparentHeader;

        public ICommand ChangeColourCommand => new RelayCommand(ChangeColour);

		private void ChangeColour()
		{
			//ThemeColourProperty = dialogueService.ShowColourPickerDialogue(
   //             ThemeColourProperty, (colour) => ThemeColourProperty = colour);
		}

		/// <summary>
		/// Gets or sets the current Theme colour.
		/// </summary>
		public Color ThemeColourProperty
		{
			get => themeColour;
            set
            {
                SetProperty(ref themeColour, value);
                ThemeManager.SetTheme(value);
            }
		}

        /// <summary>
        /// Gets or sets whether the theme is being synchronised with the OS.
        /// </summary>
        public bool IsSyncingWithOs
		{
			get => isSyncingWithOs;
			set
			{
				SetProperty(ref isSyncingWithOs, value);
                ThemeManager.SyncThemeWithOs(value);
                SyncWithOsChanged?.Invoke(this, value);
            }
		}

        /// <summary>
        /// Gets or sets whether the theme is Dark or Light.
        /// </summary>
        public bool IsDarkMode
		{
			get => isDarkMode;
			set
			{
				SetProperty(ref isDarkMode, value);
                ThemeManager.SetDarkMode(value);
                IsDarkChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Gets or sets whether the theme is being synchronised with the OS.
        /// </summary>
        public bool IsTransparentHeader
        {
            get => isTransparentHeader;
            set
            {
                SetProperty(ref isTransparentHeader, value);
                TransparentHeaderChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Initialises a new <see cref="ThemingControlViewModel"/>.
        /// </summary>
        public ThemingControlViewModel()
        {
			//dialogueService = new DialogueService();
		}
	}
}
