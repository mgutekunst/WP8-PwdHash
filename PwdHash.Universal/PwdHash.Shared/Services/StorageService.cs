using System;
using System.Diagnostics;
using Windows.Security.Credentials;
using Newtonsoft.Json;
using PwdHash.Common;
using PwdHash.Interfaces;

namespace PwdHash.Services
{
    class StorageService : IStorageService
    {
        public void SaveToPasswordVault(string key, object obj)
        {
            var vault = new PasswordVault();

            // sererialze obj
            var serializedObj = JsonConvert.SerializeObject(obj);
            var credential = new PasswordCredential(Statics.PASSWORDVAULT_RESOURCENAME, key, serializedObj);

            vault.Add(credential);
        }

        public T GetFromPasswordVault<T>(string key)
        {
            var vault = new PasswordVault();

            try
            {
                var credentials = vault.Retrieve(Statics.PASSWORDVAULT_RESOURCENAME, key);
                
                // deserialize
                var obj = JsonConvert.DeserializeObject<T>(credentials.Password);
                return obj;

            }
            catch (Exception e)
            {

                Debug.WriteLine("StorageService.cs | GetFromPasswordVault | " + e.Message);
                return default(T);
            }
        }
    }
}
