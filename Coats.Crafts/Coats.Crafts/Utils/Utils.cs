using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Web.Security;
using System.IO;
using System.Text;
using System.Security.Cryptography;
namespace Coats.Crafts.Utils
{
    public static class General
    {
        public static string AdjustUrl(this string url)
        {
            return AdjustUrlToContext(url);
        }

        public static string AdjustUrlToContext(string url)
        {
            string appPath = HttpContext.Current.Request.ApplicationPath;

            //var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            //var logger = accessor.Container.Resolve<ILogger>();
            //logger.DebugFormat("AdjustUrlToContext url: {0} appPath: {1}", url, appPath);

            if (string.IsNullOrEmpty(appPath) || appPath.Equals("/"))
                return url;

            if (url.StartsWith("http"))
                return url;

            if (url.StartsWith("mailto"))
                return url;

            if (url.StartsWith("ftp"))
                return url;
            
            if (url.StartsWith(appPath))
            {
                return url;
            } else {
                return appPath + url;
            }
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            try
            {
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }
            catch(Exception Ex)
            {
            
            }
            return cipherText;
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
    }
}