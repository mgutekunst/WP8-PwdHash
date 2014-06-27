using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PwdHash.Utils;
using PwdHash.WinStore.Model;

namespace PwdHash.WinStore.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private const int MaxRecentHashes = 20;

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

        private ObservableCollection<Hash> _favorites;
        public ObservableCollection<Hash> Favorites
        {
            get { return _favorites; }
            set { _favorites = value; RaisePropertyChanged("Favorites"); }
        }

        #endregion



        #region Commands

        public RelayCommand HashCommand { get; private set; }
        public RelayCommand AddToFavoritesCommand { get; private set; }

        #endregion


        public MainViewModel()
        {

            HashCommand = new RelayCommand(hash);
            AddToFavoritesCommand = new RelayCommand(addToFavorites);

            RecentHashes = new ObservableCollection<Hash>();
            Favorites = new ObservableCollection<Hash>();
            
            if (IsInDesignModeStatic)
            {
                createDesignTimeData();
            }
        }


        private void hash()
        {
            if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(Password))
                return;

            var h = HashPassword.create(Password, Url);
            Hash = h;

            addToRecentHashes(new Hash{ Password = Password, Url = Url });
        }

        private void addToFavorites()
        {
            if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(Password))
                return;

            var hash = new Hash { Url = Url, Password = Password };
            Favorites.Add(hash);
        }

        private void addToRecentHashes(Hash hash)
        {
            RecentHashes.Add(hash);
            if (RecentHashes.Count > MaxRecentHashes)
            {
                RecentHashes.RemoveAt(0);
            }

            // todo : Save 
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
