using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class BinIcon : BaseIcon
	{
		static BinIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BinIcon), new FrameworkPropertyMetadata(typeof(BinIcon)));
		}
	}
}
