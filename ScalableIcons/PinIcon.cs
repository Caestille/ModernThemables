using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class PinIcon : BaseIcon
	{
		static PinIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PinIcon), new FrameworkPropertyMetadata(typeof(PinIcon)));
		}
	}
}
