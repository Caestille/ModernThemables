using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class MenuIcon : BaseIcon
	{
		static MenuIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuIcon), new FrameworkPropertyMetadata(typeof(MenuIcon)));
		}
	}
}
