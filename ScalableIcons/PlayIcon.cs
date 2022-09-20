using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class PlayIcon : BaseIcon
	{
		static PlayIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PlayIcon), new FrameworkPropertyMetadata(typeof(PlayIcon)));
		}
	}
}
