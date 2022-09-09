using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class GraphIcon : BaseIcon
	{
		static GraphIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphIcon), new FrameworkPropertyMetadata(typeof(GraphIcon)));
		}
	}
}
