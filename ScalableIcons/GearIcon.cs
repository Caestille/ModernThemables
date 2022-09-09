using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class GearIcon : BaseIcon
	{
		static GearIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GearIcon), new FrameworkPropertyMetadata(typeof(GearIcon)));
		}
	}
}
