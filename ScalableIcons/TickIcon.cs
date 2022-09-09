using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class TickIcon : BaseIcon
	{
		static TickIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TickIcon), new FrameworkPropertyMetadata(typeof(TickIcon)));
		}
	}
}
