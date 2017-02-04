using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Popups;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PwdHash.Common;
using PwdHash.Interfaces;
using PwdHash.Utils;
using PwdHash.WinStore.Model;

namespace PwdHash.WinStore.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private const int MaxRecentHashes = 20;

        private IStorageService _storageService;

        #region Properties

        private string _url;

        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                RaisePropertyChanged("Url");
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged("Password");
            }
        }

        private string _hash;

        public string Hash
        {
            get { return _hash; }
            set
            {
                _hash = value;
                RaisePropertyChanged("Hash");
            }
        }

        private ObservableCollection<Hash> _recentHashes;

        public ObservableCollection<Hash> RecentHashes
        {
            get { return _recentHashes; }
            set
            {
                _recentHashes = value;
                RaisePropertyChanged("RecentHashes");
            }
        }

        private ObservableCollection<Hash> _selectedRecentHashes;
        public ObservableCollection<Hash> SelectedRecentHashes
        {
            get { return _selectedRecentHashes; }
            set { _selectedRecentHashes = value; RaisePropertyChanged("SelectedRecentHashes"); }
        }

        private ObservableCollection<Hash> _favorites;
        public ObservableCollection<Hash> Favorites
        {
            get { return _favorites; }
            set { _favorites = value; RaisePropertyChanged("Favorites"); }
        }

        private ObservableCollection<Hash> _selectedFavoriteHashes;
        public ObservableCollection<Hash> SelectedFavoriteHashes
        {
            get { return _selectedFavoriteHashes; }
            set { _selectedFavoriteHashes = value; RaisePropertyChanged("SelectedFavoriteHashes"); }
        }

        #endregion



        #region Commands

        public RelayCommand HashCommand { get; private set; }
        public RelayCommand AddToFavoritesCommand { get; private set; }
        public RelayCommand<Hash> ListItemTappedCommand { get; private set; }
        public RelayCommand DeleteItemsCommand { get; private set; }

        #endregion


        public MainViewModel(IStorageService storageService)
        {
            _storageService = storageService;


            HashCommand = new RelayCommand(hash);
            AddToFavoritesCommand = new RelayCommand(addToFavorites);
            ListItemTappedCommand = new RelayCommand<Hash>(listItemTapped);
            DeleteItemsCommand = new RelayCommand(deleteItems);

            if (IsInDesignModeStatic)
            {
                createDesignTimeData();
            }
            else
            {
                // get recentHashes and Favorites
                var recent = _storageService.GetFromPasswordVault<ObservableCollection<Hash>>(Statics.MAINVIEWMODEL_KEY_RECENT);
                var favorites = _storageService.GetFromPasswordVault<ObservableCollection<Hash>>(Statics.MAINVIEWMODEL_KEY_FAVORITES);

                RecentHashes = recent ?? new ObservableCollection<Hash>();
                Favorites = favorites ?? new ObservableCollection<Hash>();
            }

        }

        private void deleteItems()
        {
            if (SelectedFavoriteHashes != null)
            {
                foreach (var item in SelectedFavoriteHashes)
                {
                    Favorites.Remove(item);
                }
                SelectedFavoriteHashes.Clear();
            }

            if (SelectedRecentHashes != null)
            {
                foreach (var item in SelectedRecentHashes)
                {
                    RecentHashes.Remove(item);
                }
                SelectedRecentHashes.Clear();
            }
        }

        private void deleteHash(Hash obj)
        {
            // find according list
            if (Favorites.Contains(obj))
                deleteFromFavorites(obj);
            else if (RecentHashes.Contains(obj))
                deleteFromRecentHashes(obj);
            else
                Debug.Assert(false, "Couldn't find Hash in any list");

            // delete
        }

        private void deleteFromRecentHashes(Hash hash)
        {
            RecentHashes.Remove(hash);
            _storageService.SaveToPasswordVault(Statics.MAINVIEWMODEL_KEY_RECENT, RecentHashes);
        }

        private void deleteFromFavorites(Hash hash)
        {
            Favorites.Remove(hash);
            _storageService.SaveToPasswordVault(Statics.MAINVIEWMODEL_KEY_FAVORITES, Favorites);
        }


        private void hash()
        {
            if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(Password))
                return;

            if (doHash(Url, Password))
            {
                var extractedUrl = DomainExtractor.Extract(Url);
                if(RecentHashes.All(r => r.Url != extractedUrl))
                {
                    addToRecentHashes(new Hash { Password = Password, Url = extractedUrl });
                }
            }
        }

        private bool doHash(string url, string password)
        {
            var extractedUrl = DomainExtractor.Extract(url);

            try
            {
                Hash = HashPassword.create(password, extractedUrl);
            }
            catch (Exception e)
            {
                Debug.WriteLine("MainViewModel.cs | doHash | " + e.Message);
                Hash = String.Empty;

                return false;
            }
            return true;
        }

        private void addToFavorites()
        {
            if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(Password))
                return;

            var extractedUrl = DomainExtractor.Extract(Url);

            if (Favorites.Any(a => a.Url == extractedUrl))
            {
                var res = new Windows.ApplicationModel.Resources.ResourceLoader();
                var message = new MessageDialog(res.GetString("MainPageMessageAlreadyFavoriteContent"), res.GetString("MainPageMessageAlreadyFavoriteTitle"));
                message.ShowAsync();

                return;
            }

            var hash = new Hash { Url = extractedUrl, Password = Password };
            Favorites.Add(hash);

            _storageService.SaveToPasswordVault(Statics.MAINVIEWMODEL_KEY_FAVORITES, Favorites);
        }

        private void addToRecentHashes(Hash hash)
        {
            RecentHashes.Add(hash);
            if (RecentHashes.Count > MaxRecentHashes)
            {
                RecentHashes.RemoveAt(0);
            }

            _storageService.SaveToPasswordVault(Statics.MAINVIEWMODEL_KEY_RECENT, RecentHashes);
        }

        private void listItemTapped(Hash item)
        {
            Url = item.Url;
            Password = item.Password;
            doHash(Url, Password);
        }


        private void createDesignTimeData()
        {
            Url = "http://www.pwdhash.com";
            Password = "geheim";
            Hash = "aXnrdue8";

            RecentHashes = new ObservableCollection<Hash>
					{
						new Hash{Url = "http://www.google.de", Password = "geheim"},
						new Hash{Url = "http://www.google.de", Password = "geheim"},
						new Hash{Url = "http://www.google.de", Password = "geheim"},
					};

            Favorites = new ObservableCollection<Hash>
					{
						new Hash{Url = "http://www.google.de", Password = "geheim"},
						new Hash{Url = "http://www.google.de", Password = "geheim"},
						new Hash{Url = "http://www.google.de", Password = "geheim"},
					};

        }
    }
}
