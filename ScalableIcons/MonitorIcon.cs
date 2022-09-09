using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class MonitorIcon : BaseIcon
	{
		static MonitorIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MonitorIcon), new FrameworkPropertyMetadata(typeof(MonitorIcon)));
		}
	}
}
