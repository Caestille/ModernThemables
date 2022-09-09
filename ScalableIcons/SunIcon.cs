using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class SunIcon : BaseIcon
	{
		static SunIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SunIcon), new FrameworkPropertyMetadata(typeof(SunIcon)));
		}
	}
}
