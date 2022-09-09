using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class PlusIcon : BaseIcon
	{
		static PlusIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PlusIcon), new FrameworkPropertyMetadata(typeof(PlusIcon)));
		}
	}
}
