using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class OpenFileIcon : BaseIcon
	{
		static OpenFileIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(OpenFileIcon), new FrameworkPropertyMetadata(typeof(OpenFileIcon)));
		}
	}
}
