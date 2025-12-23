using System;
using System.Security.Cryptography;

namespace MyCommLib.Classes
{
    public static class clsAccountHash
    {
        public static string GetHashedUserId(string user, DateTime date)
        {
            string salt = $"{date.ToString("yyyyMMdd")}";
            return clsSaltAndHash.GetHD(user, salt).Substring(10, 20);
        }
        public static string GetSecretCode(string user, DateTime date) {
            string salt = $"{date.ToString("yyyyMMdd")}";
            return clsSaltAndHash.Get(user, salt).Substring(10, 8);
        }
        public static string GetHashedPassword(string email)
        {
            string salt = $"This is just to make it more random";
            return clsSaltAndHash.Get(email, salt);
        }
    }
}
