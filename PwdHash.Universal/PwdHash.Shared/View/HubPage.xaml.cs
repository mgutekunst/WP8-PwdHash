using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private bool isMovedUp, isInEditingMode;

        ListView _favorites, _recent;


        MainViewModel _vm;

        public HubPage()
        {
            this.InitializeComponent();

            _vm = (MainViewModel)DataContext;
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
            if (isMovedUp)
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

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ObservableCollection<Hash> list;
            var listView = sender as ListView;
            if (listView != null)
            {
                var items = listView.SelectedItems.Cast<Hash>();
                if (listView.Name == "RecentHashesListView")
                {
                    _vm.SelectedRecentHashes = new ObservableCollection<Hash>(items);
                }
                else
                {
                    _vm.SelectedFavoriteHashes = new ObservableCollection<Hash>(items);
                }

                if((_vm.SelectedRecentHashes != null && _vm.SelectedRecentHashes.Any()) || (_vm.SelectedFavoriteHashes != null && _vm.SelectedFavoriteHashes.Any()))
                {
                    DeleteAppBarButton.Visibility = Visibility.Visible;
                    FavoriteAppBarButton.Visibility = Visibility.Collapsed;
                }
                else
                {
#if WINDOWS_PHONE_APP
                    EditingAppBarButton_OnClicked(null,null);
#else
                    DeleteAppBarButton.Visibility = Visibility.Collapsed;
                    FavoriteAppBarButton.Visibility = Visibility.Visible;
                    isInEditingMode = false;
#endif
                }
            }

        }

        private void EditingAppBarButton_OnClicked(object sender, RoutedEventArgs e)
        {
            if (!isInEditingMode)
            {
                _recent.SelectionMode = ListViewSelectionMode.Multiple;
                _recent.IsItemClickEnabled = false;
                _favorites.SelectionMode = ListViewSelectionMode.Multiple;
                _favorites.IsItemClickEnabled = false;
                DeleteAppBarButton.Visibility = Visibility.Visible;
                FavoriteAppBarButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                _recent.SelectionMode = ListViewSelectionMode.None;
                _recent.IsItemClickEnabled = true;
                _favorites.SelectionMode = ListViewSelectionMode.None;
                _favorites.IsItemClickEnabled = true;
                DeleteAppBarButton.Visibility = Visibility.Collapsed;
                FavoriteAppBarButton.Visibility = Visibility.Visible;
            }

            isInEditingMode = !isInEditingMode;
        }

        private void RecentHashesListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _recent = sender as ListView;
#if WINDOWS_PHONE_APP
            if(_recent != null)
            {
                _recent.SelectionMode = ListViewSelectionMode.None;
            }
#endif
        }

        private void FavoritesListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _favorites = sender as ListView;

#if WINDOWS_PHONE_APP
            if (_favorites != null)
            {
                _favorites.SelectionMode = ListViewSelectionMode.None;
            }
#endif

        }

        private void EditingAppBarButton_OnLoaded(object sender, RoutedEventArgs e)
        {
#if WINDOWS_APP
            EditingAppBarButton.Visibility = Visibility.Collapsed;
#endif
        }
    }
}