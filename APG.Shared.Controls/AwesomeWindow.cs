using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace APG.Shared
{
    public abstract class AwesomeWindow : SimpleWindow
    {
        static AwesomeWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AwesomeWindow), new FrameworkPropertyMetadata(typeof(AwesomeWindow)));
        }

        public ObservableCollection<ButtonBase> TitlebarButtons { get; } = new();

        public override void OnApplyTemplate()
        {
            if (GetTemplateChild("TitlebarButtons") is ItemsControl titlebarButtons) titlebarButtons.ItemsSource = TitlebarButtons;

            base.OnApplyTemplate();
        }
    }
}
