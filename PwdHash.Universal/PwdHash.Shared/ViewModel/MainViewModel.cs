using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PwdHash.Utils;

namespace PwdHash.WinStore.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _url;
        public string Url
        {
            get { return _url; }
            set { _url = value; RaisePropertyChanged("Url"); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; RaisePropertyChanged("Password"); }
        }

        private string _hash;
        public string Hash
        {
            get { return _hash; }
            set { _hash = value; RaisePropertyChanged("Hash"); }
        }

        public RelayCommand HashCommand { get; set; }
        public MainViewModel()
        {
            if(IsInDesignModeStatic)
            {
                createDesignTimeData();
            }

            HashCommand = new RelayCommand(hash);
        }

        private void hash()
        {
            if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(Password))
                return;

            var h = HashPassword.create(Password, Url);
            Hash = h;
        }

        private void createDesignTimeData()
        {
            Url = "http://www.pwdash.com";
            Password = "geheim";
            Hash = "aXnrdue8";
        }
    }
}
