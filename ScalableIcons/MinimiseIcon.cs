using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class MinimiseIcon : BaseIcon
	{
		static MinimiseIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MinimiseIcon), new FrameworkPropertyMetadata(typeof(MinimiseIcon)));
		}
	}
}
