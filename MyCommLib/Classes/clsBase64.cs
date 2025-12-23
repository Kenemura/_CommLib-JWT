namespace MyCommLib.Classes;

using System;

public class clsBase64
{
    public static string Decode(string encoded)
    {
        byte[] b;
        string decoded;
        try
        {
            b = Convert.FromBase64String(encoded);
            decoded = System.Text.ASCIIEncoding.ASCII.GetString(b);
        }
        catch (FormatException)
        {
            decoded = "";
        }
        return decoded;
    }

    public static string Encode(string original)
    {
        byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(original);
        string encoded = Convert.ToBase64String(b);
        return encoded;
    }

}
