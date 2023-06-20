using ModernThemables.Charting.Models;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using ModernThemables.Charting.ViewModels;
using System.Collections.Generic;
using System.Linq;
using ModernThemables.Charting.Services;
using System.Threading.Tasks;

namespace ModernThemables.Charting.Controls.ChartComponents
{
	/// <summary>
	/// Interaction logic for ZoomHost.xaml
	/// </summary>
	public partial class ZoomHost : UserControl
	{
		public MouseCoordinator Coordinator
		{
			get => (MouseCoordinator)GetValue(MouseCoordinatorProperty);
			set => SetValue(MouseCoordinatorProperty, value);
		}
		public static readonly DependencyProperty MouseCoordinatorProperty = DependencyProperty.Register(
			"MouseCoordinator",
			typeof(MouseCoordinator),
			typeof(ZoomHost),
			new PropertyMetadata(null, OnSetMouseCoordinator));

		public ZoomHost()
		{
			InitializeComponent();
			this.Loaded += ZoomHost_Loaded;
		}

		private void ZoomHost_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= ZoomHost_Loaded;
			if (ChartHelper.FindMouseCoordinatorFromVisualTree(this, out var coordinator))
			{
				Coordinator = coordinator;
			}
			else
			{
				throw new InvalidOperationException("Please add a MouseCoordinator to your chart");
			}
		}

		private static async void OnSetMouseCoordinator(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not ZoomHost _this) return;
		}
	}
}
