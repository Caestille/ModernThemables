using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class CalendarIcon : BaseIcon
	{
		static CalendarIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarIcon), new FrameworkPropertyMetadata(typeof(CalendarIcon)));
		}
	}
}
