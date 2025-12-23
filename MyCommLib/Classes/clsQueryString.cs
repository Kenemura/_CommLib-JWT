using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace MyCommLib.Classes;

public class clsQueryString
{
    private NavigationManager NM;
    private string QueryStrings;
    public clsQueryString(NavigationManager nm)
    {
        NM = nm;
        QueryStrings = NM.ToAbsoluteUri(NM.Uri).Query;
    }
    public string GetValue(string name)
    {
        StringValues val = "";
        if (QueryHelpers.ParseQuery(QueryStrings).TryGetValue(name, out val))
        {
            return val!;
        }
        return "";
    }
    public string GetEncoded(string str)
    {
        return System.Net.WebUtility.UrlEncode(str);
    }
    public string GetDecoded(string str)
    {
        return System.Net.WebUtility.UrlDecode(str);
    }
}
