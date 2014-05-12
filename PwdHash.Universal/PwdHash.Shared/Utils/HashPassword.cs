using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace PwdHash.Utils
{
    public static class HashPassword
    {
        private static int passwordPrefix;
        private const string _hashAlgorithm = "Md5";
        public static string create(string password, string url)
        {
            if(string.IsNullOrEmpty(password) || string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("password and url must not be empty");
            }
            
            var res = hash(password, url);

            var resString = CryptographicBuffer.EncodeToBase64String(res);
            passwordPrefix = 2;
            int length = password.Length + passwordPrefix;

            return resString.Substring(0, length);
        }

        private static IBuffer hash(string key, string secret)
        {
            var mac = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacMd5);
            var buf = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);

            var b = mac.CreateKey(buf);

            CryptographicHash cHash = mac.CreateHash(CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8));
            cHash.Append(CryptographicBuffer.ConvertStringToBinary(secret, BinaryStringEncoding.Utf8));

            var res = cHash.GetValueAndReset();
            return res;
        }
    }
}