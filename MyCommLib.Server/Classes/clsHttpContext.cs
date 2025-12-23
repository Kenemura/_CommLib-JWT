using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MyCommLib.Server.Classes;
public class clsHttpContext
{
    private readonly HttpContext _context;
    public clsHttpContext(HttpContext context)
    {
        _context = context;
    }
    public static string GetBaseUrl(HttpContext context)
    {
        var s = $"{context.Request.Scheme}://{context.Request.Host.Value}";
        if (context.Request.PathBase.HasValue)
        {
            s += $"{context.Request.PathBase.Value}";
        }
        return $"{s}/";
    }
    public static string GetUrl(HttpContext context)
    {
        return GetBaseUrl(context) + context.Request.Path;
    }
    //public static string GetCookie(string key)
    //{
    //    return "";
    //}
    public string UserName => _context.User.Identity?.Name ?? "";
    public string UserEmail => _context.User.FindFirst(x => x.Type == ClaimTypes.Email)?.Value ?? "";
    public int CookieCount => _context.Request.Cookies.Count;
    public KeyValuePair<string, string> Cookie(string name) => _context.Request.Cookies.FirstOrDefault(x => x.Key == name);
    public KeyValuePair<string, string> Cookie(int no) => _context.Request.Cookies.Skip(no--).FirstOrDefault();
    public string SessionId => (_context.Session is null) ? "" : _context.Session.Id;
}
