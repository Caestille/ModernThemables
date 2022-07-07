using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Win10Themables.Controls
{
	public partial class ThemingControl : UserControl
	{
		public event EventHandler InternalRequestClose;

		public ThemingControl()
		{
			InitializeComponent();
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
			SyncCheckbox.Focus();
		}
	}
}