using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace WebApiNew.Controllers
{
    public class CipherController : ApiController
    {
        const string IV = "qjdmzsfepyensjhg";
        [Route("api/sifrele")]
        [HttpGet]
        public string encryptText(string text, string key)
        {
            key = getUsableKey(key);
            return Encrypt(text, key);
        }

        [Route("api/sifreCoz")]
        [HttpGet]
        public string decryptText(string cipher, string key)
        {
            key = getUsableKey(key);
            return Decrypt(cipher, key);
        }

        private string getUsableKey(string key)
        {
            if (key == null || String.IsNullOrWhiteSpace(key))
                throw new Exception("A key must be provided!");
            if (key.Length > 16)
                key = key.Substring(0, 16);
            else if (key.Length < 16)
            {
                while (key.Length < 16)
                {
                    key = key + "k";
                }

            }
            return key;
        }

        private static string Encrypt(string text, string Key)
        {
            AesCryptoServiceProvider aes=null;
            ICryptoTransform crypto=null;
            try
            {
                byte[] plaintextbytes = System.Text.Encoding.ASCII.GetBytes(text);
                aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 128;
                aes.Key = System.Text.Encoding.ASCII.GetBytes(Key);
                aes.IV = System.Text.Encoding.ASCII.GetBytes(IV);
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                crypto = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] encrypted = crypto.TransformFinalBlock(plaintextbytes, 0, plaintextbytes.Length);
                crypto.Dispose();
                aes.Clear();
                aes.Dispose();
                return Convert.ToBase64String(encrypted);
            }
            finally
            {
                crypto?.Dispose();
                if (aes != null)
                {
                    aes.Clear();
                    aes.Dispose();
                }

            }
        }

        public static string Decrypt(string cipher, string Key)
        {
            AesCryptoServiceProvider aes=null;
            ICryptoTransform crypto=null;
            try
            {
                aes = new AesCryptoServiceProvider();
                byte[] encryptedbytes = Convert.FromBase64String(cipher);
                aes.BlockSize = 128;
                aes.KeySize = 128;
                aes.Key = System.Text.Encoding.ASCII.GetBytes(Key);
                aes.IV = System.Text.Encoding.ASCII.GetBytes(IV);
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                crypto = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] secret = crypto.TransformFinalBlock(encryptedbytes, 0, encryptedbytes.Length);
                crypto.Dispose();
                aes.Clear();
                aes.Dispose();
                return System.Text.Encoding.ASCII.GetString(secret);
            }
            finally
            {
                crypto?.Dispose();
                if (aes != null)
                {
                    aes.Clear();
                    aes.Dispose();
                }

            }

        }

    }
}
