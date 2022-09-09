using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class ExpandIcon : BaseIcon
	{
		static ExpandIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpandIcon), new FrameworkPropertyMetadata(typeof(ExpandIcon)));
		}
	}
}
