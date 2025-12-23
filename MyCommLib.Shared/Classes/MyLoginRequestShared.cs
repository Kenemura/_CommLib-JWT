namespace MyCommLib.Shared.Classes;

using MyCommLib.Classes;
using MyCommLib.Shared;
using System;

public class MyLoginRequestShared
{
    public static string GetHashedUserId(string user, DateTime date)
    {
        string salt = $"{date.ToString("yyyyMMdd")}";
        return clsSaltAndHash.GetHD(user, salt).Substring(10, 20);
    }
    public static string GetHashedPassword(string email)
    {
        string salt = $"This is just to make it more random";
        return clsSaltAndHash.Get(email, salt);
    }

    public static string EmailBody(string username, string email)
    {
        string url = $"{MyAppInfo.AppUrl.ToLower()}" + AutoLoginPath(username, email);

        string body = $"<h3>Hello {username}-san!</h3>";
        body += $@"<p>You can login to the site by using this <a href=""{url}"">Link</a>.";
        body += "<br/>(The above link works only upto 24hours for security reason)</p>";
        body += EmailFooter();
        body = "<font size=\"+1\">" + body + "</font>";
        return body;
    }
    public static string AutoLoginPath(string username, string email)
    {
        string path = $"MyAccount/AutoLogin";
        path += $"?Username={username}&Email={email}";
        path += $"&UserId={GetHashedUserId(email, clsLocalTime.Today())}";
        return path;
    }
    private static string EmailFooter()
    {
        var ft = "<br/>";
        ft += "==================================================<br/>";
        ft += $"{MyAppInfo.AppName}<br/>";
        ft += $"URL: {MyAppInfo.AppUrl}<br/>";
        ft += "==================================================<br/>";
        return ft;
    }
}
