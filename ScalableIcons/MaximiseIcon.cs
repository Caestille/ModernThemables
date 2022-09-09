using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class MaximiseIcon : BaseIcon
	{
		static MaximiseIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MaximiseIcon), new FrameworkPropertyMetadata(typeof(MaximiseIcon)));
		}
	}
}
