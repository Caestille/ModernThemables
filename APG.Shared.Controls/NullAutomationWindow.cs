using System.Windows;
using System.Windows.Automation.Peers;

namespace APG.Shared;

public class NullAutomationWindow : Window
{
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new NullAutomationPeer(this);
    }
}
