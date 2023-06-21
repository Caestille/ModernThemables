using System.Windows;
using System.Windows.Controls;

namespace APG.Shared
{
    public class AwesomeConsole : Control
    {
        static AwesomeConsole()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AwesomeConsole), new FrameworkPropertyMetadata(typeof(AwesomeConsole)));
        }

        #region Messages

        public static readonly DependencyProperty MessagesProperty = DependencyProperty.Register
        (
            "Messages",
            typeof(IEnumerable<string>),
            typeof(AwesomeConsole),
            new FrameworkPropertyMetadata(null, OnMessagesPropertyChanged)
        );

        private static void OnMessagesPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (source is AwesomeConsole awesomeConsole) awesomeConsole.UpdateItemsSource();
        }

        public IEnumerable<string>? Messages
        {
            get { return (IEnumerable<string>?)GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        #endregion

        private void UpdateItemsSource()
        {
            if (GetTemplateChild("ItemsControl") is ItemsControl itemsControl) itemsControl.ItemsSource = Messages;
        }

        public override void OnApplyTemplate()
        {
            UpdateItemsSource();

            base.OnApplyTemplate();
        }
    }
}
