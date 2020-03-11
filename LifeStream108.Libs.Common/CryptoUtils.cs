using System;
using System.Security.Cryptography;
using System.Text;

namespace LifeStream108.Libs.Common
{
    public static class CryptoUtils
    {
        public static string GenerateSha256Hash(string value)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }
    }
}
