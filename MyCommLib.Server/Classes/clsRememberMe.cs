namespace MyCommLib.Server.Classes;

using MyCommLib.Shared;
public class clsRememberMe
{
    static string cPassword = $"Himitsu {MyAppInfo.AppTitle}";
    public static string GetEncrypted(string original)
    {
        var encrypted = clsEncryption.Encrypt(TimeStamp + original, cPassword);
        return encrypted;
    }
    public static string GetDecrypted(string encrypted)
    {
        var decrypted = clsEncryption.Decrypt(encrypted, cPassword);
        decrypted = decrypted.Substring(TimeStamp.Length, decrypted.Length - TimeStamp.Length);
        return decrypted;
    }

    static string cFormat = "HH:mm:ss ddd dd MMM yyyy";
    private static string TimeStamp => DateTime.Now.ToString(cFormat);
}
