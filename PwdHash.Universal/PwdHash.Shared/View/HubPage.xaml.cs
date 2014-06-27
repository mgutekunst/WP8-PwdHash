using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PwdHash.WinStore.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private PasswordBox _passwordBox;
        private bool isMovedUp;

        public HubPage()
        {
            this.InitializeComponent();
            var rl = new ResourceLoader();

            var s = rl.GetString("ApplicationTitle");
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
        }
    }
}