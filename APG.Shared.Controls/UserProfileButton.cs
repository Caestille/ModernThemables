using System.Windows;
using System.Windows.Controls;

namespace APG.Shared
{
    public class UserProfileButton : Button
    {
        static UserProfileButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UserProfileButton), new FrameworkPropertyMetadata(typeof(UserProfileButton)));
        }

        #region Username

        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register
        (
            "Username",
            typeof(string),
            typeof(UserProfileButton),
            new FrameworkPropertyMetadata(null, OnUsernamePropertyChanged)
        );

        private static void OnUsernamePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (source is UserProfileButton userProfileButton) userProfileButton.Content = userProfileButton.Username;
        }

        public string? Username
        {
            get { return (string?)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        #endregion
    }
}
