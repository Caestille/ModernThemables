namespace ModernThemables.Controls;

using System;
using System.Windows.Controls;
using System.Windows.Input;

public partial class ThemingControl : UserControl
{
    public ThemingControl()
    {
        this.InitializeComponent();
    }

    public event EventHandler? InternalRequestClose;

    public void FocusOnOpen() => this.SyncCheckbox.Focus();

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.InternalRequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
