using System;
using System.Collections.Generic;
using System.Text;

namespace PwdHash.WinStore.Model
{
    public class Hash
    {
        public string Url { get; set; }
        public string Password { get; set; }

        public Hash()
        {
            
        }

        public Hash(string url, string password)
        {
            Url = url;
            Password = password;
        }
    }
}
