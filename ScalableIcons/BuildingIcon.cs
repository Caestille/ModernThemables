using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class BuildingIcon : BaseIcon
	{
		static BuildingIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BuildingIcon), new FrameworkPropertyMetadata(typeof(BuildingIcon)));
		}
	}
}
