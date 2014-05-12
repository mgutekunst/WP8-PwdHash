using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace PwdHash.Utils
{
    public static class HashPassword
    {
        /// <summary>
        /// Password Prefix, normally this as @@
        /// </summary>
        private static int passwordPrefix = 2;
        
        /// <summary>
        /// Regex to find non Alphanumerical characters
        /// </summary>
        static Regex _nonNumeric = new Regex("[^a-zA-Z0-9_]");

        private static List<char> _mExtras;

        /// <summary>
        /// Create a hash based on <paramref name="password"/> and <paramref name="url"/>
        /// </summary>
        public static string create(string password, string url)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("password and url must not be empty");
            }

            var res = hash(password, url);

            var hashString = CryptographicBuffer.EncodeToBase64String(res);
            int size = password.Length + passwordPrefix;
            bool nonAlphanumeric = _nonNumeric.IsMatch(password);

            return applyConstraints(hashString, size, nonAlphanumeric);
        }

        private static IBuffer hash(string key, string secret)
        {
            var mac = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacMd5);

            CryptographicHash cHash = mac.CreateHash(encodeStringToBytes(key).AsBuffer());
            cHash.Append(encodeStringToBytes(secret).AsBuffer());

            var res = cHash.GetValueAndReset();
            return res;
        }

        /// <summary>
        /// Returns a new byte array with the encoded input string (1 byte per
        /// character).
        /// 
        /// Characters in the Latin 1 range (up to code point 255) will be returned
        /// as Latin 1 encoded bytes. Characters above code point 255 will be
        /// UTF-16le encoded but only the first byte will be used.
        /// 
        /// This matches the original behavior of the PwdHash JavaScript
        /// implementation pwdhash.com and keeps the hash values of passwords
        /// containing non-latin1 characters compatible.
        /// </summary>
        /// <param name="data">input string</param>
        private static byte[] encodeStringToBytes(String data)
        {
            byte[] bytes = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                var ch = data[i];

                if (ch <= 255)
                {
                    bytes[i] = (byte)ch;
                }
                else
                {
                    try
                    {
                        var charBytes = CryptographicBuffer.ConvertStringToBinary(ch.ToString(), BinaryStringEncoding.Utf16LE);
                        short unsignedByte = (short)(0x000000FF & (charBytes.ToArray()[0]));
                        bytes[i] = (byte)unsignedByte;
                    }
                    catch (Exception)
                    {

                        Debug.WriteLine("HashPassword.cs | encodeStringToBytes | DecodingFailed with char {0}", ch); ;
                        bytes[i] = 0x1A; // SUB
                    }
                }
            }
            return bytes;
        }

        private static string applyConstraints(string hash, int size, bool nonAlphanumeric)
        {
            int startingSize = size - 4;
            if (startingSize < 0)
                startingSize = 0;
            else if (startingSize > hash.Length)
                startingSize = hash.Length;

            var result = hash.Substring(0, startingSize);

            _mExtras = hash.Substring(startingSize).ToList();

            result += Regex.IsMatch(result, "[A-Z]") ? nextExtraChar() : nextBetween('A', 26);
            result += Regex.IsMatch(result, "[a-z]") ? nextExtraChar() : nextBetween('a', 26);
            result += Regex.IsMatch(result, "[0-9]") ? nextExtraChar() : nextBetween('0', 10);
            result += _nonNumeric.IsMatch(result) && nonAlphanumeric ? nextExtraChar() : '+';

            while (_nonNumeric.IsMatch(result) && !nonAlphanumeric)
            {
                String replacement = nextBetween('A', 26).ToString();
                result = _nonNumeric.Replace(result, replacement, 1);
            }

            // For long passwords (about > 22 chars) the password might be longer
            // than the hash and mExtras is empty. In that case the constraints
            // above produce 0 bytes at the end of result. If nonAlphanumeric is not
            // set those 0 bytes are replaced, but in other cases they stay around
            // and must be removed here. This is a flaw in the original algorithm
            // which we have to work around here.
            result = result.Replace("\0", "");

            // because base64 encoded strings end with '==', we also replace those
            result = result.Replace("=", "");

            // Rotate the result to make it harder to guess the inserted locations
            return rotate(result, nextExtra());
        }

        private static int nextExtra()
        {
            return (int)nextExtraChar();
        }

        private static char nextExtraChar()
        {
            char res;
            if (_mExtras.Count > 0)
            {
                res = _mExtras[0];
                _mExtras.RemoveAt(0);
            }
            else
            {
                res = '\0';
            }
            return res;
        }

        private static int between(int min, int interval, int offset)
        {
            return min + offset % interval;
        }

        private static char nextBetween(char b, int interval)
        {
            return (char)between(b, interval, nextExtra());
        }

        private static string rotate(string s, int amount)
        {
            var work = new List<char>(s.ToCharArray());

            while (amount-- > 0)
            {
                char c = work[0];
                work.RemoveAt(0);
                work.Add(c);
            }

            return string.Join("", work);
        }
    }
}