namespace ModernThemables.Controls;

using System;
using System.Windows.Controls;
using System.Windows.Input;

public partial class ThemingControl : UserControl
{
    public event EventHandler? InternalRequestClose;

    public ThemingControl()
    {
        this.InitializeComponent();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            InternalRequestClose?.Invoke(this, EventArgs.Empty);
        }
    }

    public void FocusOnOpen()
    {
        this.SyncCheckbox.Focus();
    }
}