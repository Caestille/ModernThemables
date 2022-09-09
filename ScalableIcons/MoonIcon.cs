using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class MoonIcon : BaseIcon
	{
		static MoonIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MoonIcon), new FrameworkPropertyMetadata(typeof(MoonIcon)));
		}
	}
}
