using System.Windows;
using System.Windows.Automation.Peers;

namespace APG.Shared;

public class NullAutomationPeer : FrameworkElementAutomationPeer
{
    public NullAutomationPeer(FrameworkElement owner)
        : base(owner)
    { }

    protected override string GetNameCore()
    {
        return "NullAutomationPeer";
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Window;
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
        return new List<AutomationPeer>();
    }
}
