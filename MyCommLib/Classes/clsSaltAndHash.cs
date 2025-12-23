using System.Security.Cryptography;
using System.Text;

namespace MyCommLib.Classes
{
    public class clsSaltAndHash
    {
        public static string Get(string text, string salt)
        {
            var hashedBytes = clsSaltAndHash.GetBytes(text, salt);
            return Convert.ToBase64String(hashedBytes);
        }
        public static string GetHD(string text, string salt)
        {
            var hashedBytes = clsSaltAndHash.GetBytes(text, salt);
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
        public static byte[] GetBytes(string text, string salt)
        {
            var sha = SHA256.Create();
            var saltedText = text + salt;
            var hashedBytes = sha.ComputeHash(Encoding.Unicode.GetBytes(saltedText));
            return hashedBytes;
        }
    }
}
