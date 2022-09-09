using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class CollapseIcon : BaseIcon
	{
		static CollapseIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapseIcon), new FrameworkPropertyMetadata(typeof(CollapseIcon)));
		}
	}
}
