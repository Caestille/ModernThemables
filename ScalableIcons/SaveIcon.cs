using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class SaveIcon : BaseIcon
	{
		static SaveIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SaveIcon), new FrameworkPropertyMetadata(typeof(SaveIcon)));
		}
	}
}
