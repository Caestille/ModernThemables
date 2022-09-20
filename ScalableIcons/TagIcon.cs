using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class TagIcon : BaseIcon
	{
		static TagIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TagIcon), new FrameworkPropertyMetadata(typeof(TagIcon)));
		}
	}
}
