namespace Coats.Crafts.Utils
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    public static class General
    {
        public static string AdjustUrl(this string url)
        {
            return AdjustUrlToContext(url);
        }

        public static string AdjustUrlToContext(string url)
        {
            string applicationPath = HttpContext.Current.Request.ApplicationPath;
            if (string.IsNullOrEmpty(applicationPath) || applicationPath.Equals("/"))
            {
                return url;
            }
            if (url.StartsWith("http"))
            {
                return url;
            }
            if (url.StartsWith("mailto"))
            {
                return url;
            }
            if (url.StartsWith("ftp"))
            {
                return url;
            }
            if (url.StartsWith(applicationPath))
            {
                return url;
            }
            return (applicationPath + url);
        }

        public static string Decrypt(string cipherText)
        {
            string password = "MAKV2SPBNI99212";
            try
            {
                cipherText = cipherText.Replace(" ", "+");
                byte[] buffer = Convert.FromBase64String(cipherText);
                using (Aes aes = Aes.Create())
                {
                    Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 110, 0x20, 0x4d, 0x65, 100, 0x76, 0x65, 100, 0x65, 0x76 });
                    aes.Key = bytes.GetBytes(0x20);
                    aes.IV = bytes.GetBytes(0x10);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (CryptoStream stream2 = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            stream2.Write(buffer, 0, buffer.Length);
                            stream2.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(stream.ToArray());
                    }
                    return cipherText;
                }
            }
            catch (Exception)
            {
            }
            return cipherText;
        }

        public static string Encrypt(string clearText)
        {
            string password = "MAKV2SPBNI99212";
            byte[] buffer = Encoding.Unicode.GetBytes(clearText);
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 110, 0x20, 0x4d, 0x65, 100, 0x76, 0x65, 100, 0x65, 0x76 });
                aes.Key = bytes.GetBytes(0x20);
                aes.IV = bytes.GetBytes(0x10);
                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream stream2 = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        stream2.Write(buffer, 0, buffer.Length);
                        stream2.Close();
                    }
                    clearText = Convert.ToBase64String(stream.ToArray());
                }
            }
            return clearText;
        }
    }
}

