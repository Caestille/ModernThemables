using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class CloseIcon : BaseIcon
	{
		static CloseIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CloseIcon), new FrameworkPropertyMetadata(typeof(CloseIcon)));
		}
	}
}
