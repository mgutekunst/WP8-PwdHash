﻿using System.Collections.ObjectModel;
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


        public MainViewModel(IStorageService storageService)
        {
            _storageService = storageService;


            HashCommand = new RelayCommand(hash);
            AddToFavoritesCommand = new RelayCommand(addToFavorites);



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


        private void hash()
        {
            if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(Password))
                return;

            var url = DomainExtractor.Extract(Url);

            var h = HashPassword.create(Password, url);
            Hash = h;

            addToRecentHashes(new Hash { Password = Password, Url = Url });
        }

        private void addToFavorites()
        {
            if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(Password))
                return;

            var hash = new Hash { Url = Url, Password = Password };
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
