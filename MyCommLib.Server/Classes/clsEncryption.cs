namespace MyCommLib.Server.Classes;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
public class clsEncryption
{
    private static readonly byte[] salt = Encoding.Unicode.GetBytes("Himitsu");
    private static readonly int iterations = 2000;
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static string Encrypt(string plainText, string password)
    {
        byte[] encryptedBytes;
        byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);
        using var aes = Aes.Create();
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        aes.Key = pbkdf2.GetBytes(32);
        aes.IV = pbkdf2.GetBytes(16);
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(plainBytes, 0, plainBytes.Length);
                cs.FlushFinalBlock();
            }
            encryptedBytes = ms.ToArray();
        }
        return Convert.ToBase64String(encryptedBytes);
    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static string Decrypt(string cryptoText, string password)
    {
        byte[] plainBytes;
        byte[] cryptoBytes = Convert.FromBase64String(cryptoText);
        using var aes = Aes.Create();
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        aes.Key = pbkdf2.GetBytes(32);
        aes.IV = pbkdf2.GetBytes(16);
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cryptoBytes, 0, cryptoBytes.Length);
            }
            plainBytes = ms.ToArray();
        }
        return Encoding.Unicode.GetString(plainBytes);
    }
}
