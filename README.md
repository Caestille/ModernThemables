# Win10Themables
This is a WPF app which defines a 'Window Control' which is designed to be added to the root of a WPF window in another project, reducing the overhead of repeatedly restyling your window if you want extra functionality on top of the Windows 10 functionality. The control will automatically pick up the title and icon of your window and apply it to the control.
The control is designed with built in theming, where the user can alter the 'brightness mode' (light/dark) of the window, and the window theme (accent colour). It can also optionalliy sync with the Windows 10 operating system.  Theming is altered through a build in control which drops down from a button next to the window state controls.
The window supports the usual window control behaviour such as minimising/maximising, docking and keyboard shortcuts. 
The package also implements some custom controls and styles which intrinsically match the theming of the window, unless they expose properties to assign those yourself.

# Getting started
- Add latest versions of Win10Themables and CoreUtilities nugets to your WPF project.
- Merge Styles.xaml resource dictionary from Win10Themables into a resource dictionary in your app.xaml. eg:
```
	<Application.Resources>
		<ResourceDictionary>
			<ResourceDictionary.MergedDictionaries>
				<ResourceDictionary Source="/Win10Themables;component/Styles.xaml" />
			</ResourceDictionary.MergedDictionaries>
		</ResourceDictionary>
	</Application.Resources>
```
	
- Add Win10Themables:MainWindowControl to the root of your window. Requires xaml namespace "xmlns:<Namespace Name>="clr-namespace:Win10Themables.Controls;assembly=Win10Themables"
- Set 'WindowStyle' to 'None' and 'AllowTransparency' to 'False'
- Adjust the 'Window.Chrome' 'CaptionHeight' property to '24' and the 'ResizeBorderThickness' property to a value of your choosing

	eg: 
```
	<Window
		x:Class="YourApplication.MainWindow"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:controls="clr-namespace:Win10Themables.Controls;assembly=Win10Themables"
		Title="Application Name"
		Icon="SomeIcon.ico"
		x:Name="Window"
		Width="800"
		Height="450"
		AllowsTransparency="False"
		WindowStyle="None">
		<WindowChrome.WindowChrome>
			<WindowChrome CaptionHeight="24"
						  ResizeBorderThickness="5" />
		</WindowChrome.WindowChrome>
		<controls:MainWindowControl />
	</Window>
```
	
# Notes
Controls currently include:
- Circular Progress Bar: A simple progress bar in the shape of a ring.
- Custom Slider: A slider re-styled to look more modern and accept theme colours.
- Range Slider: A slider designed to set a value range using two thumbs. Same styling as custom slider.
- Main Window Control: See above.
- Theming Control: Theme control the main window implements, but can also be added elsewhere if desired
- BlurHost: Included intrinsically from CoreUtils library. Wraps a UI element in xaml and will give appearance of blurring background behind it. Has to have a UI element given to it to blur through the 'BlurBackground' property. May also need aligning which can be done through the 'OffsetX' and 'OffsetY' properties. Accessed through the xaml namespace: "clr-namespace:CoreUtils.Controls;assembly=CoreUtils".

Styles/Control templates include:
- Custom slider thumb: Used in above slider controls. Key: "SliderThumbHorizontalDefault".
- Combobox: Key="ThemedCombobox"
- Scrollbar: Intrinisically overrides default style.
- Datagrid: Key="ThemedDataGrid"
- Listbox: Key="ThemedListbox"
- Checkbox: Key="ThemedCheckbox"
- Toggle checkbox: Apple style sliding thumb in pill shaped border. Key: "ToggleCheckbox"

Colours controls implement (accessed through a DynamicResource qualifier in xaml), and available for use in your controls include:
- MainBackgroundBrush
- TextBrush
- TextColour
- InvertedTextBrush
- StatusTextBrush
- StatusTextLightBrush
- DisabledControlClickablePartBrush
- DisabledControlNonClickablePartBrush
- ControlClickablePartBrush
- ControlClickablePartMouseOverBrush
- ControlClickablePartMousDownBrush
- ControlNonClickablePartBrush
- DatagridHeaderBrush
- DatagridRowBrush
- ThemeBrush
- ThemeMouseOverBrush
- ThemeClickBrush
- ThemeBackgroundBrush
- ThemeTextBrush
- ThemeStatusBrush
- ThemeDisabledTextBrush

Converters from Win10Themables and CoreUtilities library (added intrinsically through Win10Themables Styles.xaml) include:
- BooleanToVisibilityConverter
- VisibleIfFalseConverter
- MultiBoolAndConverter
- MultiBoolOrConverter
- StringNotEmptyVisibilityConverter
- StringWidthGetterConverter
- DateRangeFormatterConverter
- BooleanInverter
- VisibleIfOneTrueConverter
- EnumDescriptionGetterConverter
- ValueMultiplierConverter: Multiplies binding by converter parameter
- ValueAdderConverter: Adds converter parameter to binding
- DataGridWrapRowsBoolConverter
- PercentFormatterConverter
- IsNotNullConverter
- SequenceHasElementsVisibilityConverter
- DoubleInverterConverter
- ValueIfTrueValueIfFalseConverter
- RangeSliderThumbSeparationConverter
- RangeSliderMaxValueConverter
- RangeSliderThumbValueConverter