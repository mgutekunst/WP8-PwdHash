using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using PwdHash.WinStore.Model;
using PwdHash.WinStore.ViewModel;

namespace PwdHash.WinStore.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private PasswordBox _passwordBox;
        private bool isMovedUp;

        MainViewModel _vm;

        public HubPage()
        {
            this.InitializeComponent();

            _vm = (MainViewModel) DataContext;
        }

        private async void TextBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
#if WINDOWS_PHONE_APP
            if (e.Key == VirtualKey.Enter)
            {
                this.Focus(FocusState.Programmatic);
                _passwordBox.Focus(FocusState.Programmatic);
                MovePageUp.Begin();
                isMovedUp = true;
           
            }
#endif
        }


        private void PasswordBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
#if WINDOWS_PHONE_APP
            if (e.Key == VirtualKey.Enter)
            {
                this.Focus(FocusState.Programmatic);
            }
#endif
        }

        private void PasswordBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            _passwordBox = sender as PasswordBox;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
#if WINDOWS_PHONE_APP
            if(isMovedUp)
            {
                MovePageDown.Begin();
            }
            isMovedUp = false;
#endif
        }

        private void ResultBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tBox = sender as TextBox;

            tBox.Focus(FocusState.Programmatic);
            tBox.SelectAll();

            Hub.ScrollToSection(InputHubSection);
        }
    }
}