using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace APG.Shared;

class TitleBarButtonStyleSelector : StyleSelector
{
    public Style? ButtonStyle { get; set; }

    public Style? ToggleButtonStyle { get; set; }

    public override Style? SelectStyle(object item, DependencyObject container)
    {
        if (container is ToggleButton) return ToggleButtonStyle;

        return ButtonStyle;
    }
}
