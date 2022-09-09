using System.Windows;

namespace ModernThemables.ScalableIcons
{
	public class CalculatorIcon : BaseIcon
	{
		static CalculatorIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CalculatorIcon), new FrameworkPropertyMetadata(typeof(CalculatorIcon)));
		}
	}
}
