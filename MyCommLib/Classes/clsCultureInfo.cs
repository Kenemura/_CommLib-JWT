namespace MyCommLib.Classes;

using System.Globalization;
public class clsCultureInfo
{
    private CultureInfo _ci;
    public clsCultureInfo()
    {
        _ci = CultureInfo.GetCultureInfo("en-AU");
    }
    public clsCultureInfo(string code)
    {
        _ci = CultureInfo.GetCultureInfo(code);
    }
    public CultureInfo ci => _ci;
}
