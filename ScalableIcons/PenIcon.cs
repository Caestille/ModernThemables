using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class PenIcon : BaseIcon
	{
		static PenIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PenIcon), new FrameworkPropertyMetadata(typeof(PenIcon)));
		}
	}
}
