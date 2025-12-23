using Microsoft.JSInterop;

namespace MyCommLib.Classes;

public class clsCookieManager
{
    private IJSRuntime JS;
    public clsCookieManager(IJSRuntime js)
    {
        JS = js;
    }
    public async Task<string> Get(string key)
    {
        try
        {
            return await JS.InvokeAsync<string>("getCookie", key);
        }
        catch 
        {
            return string.Empty;
        }
    }
    public async Task<List<KeyValuePair<string, string>>> GetAll()
    {
        var kvps = new List<KeyValuePair<string, string>>();
        var cookies = await JS.InvokeAsync<string>("getCookies");
        if (String.IsNullOrEmpty(cookies))
        {
            return kvps;
        }
        var array = cookies.Split(";");
        foreach (var cookie in array)
        {
            var pair = cookie.Split("=");
            kvps.Add(new KeyValuePair<string, string>(pair[0], pair[1]));
        }
        return kvps;
    }
    public async Task Set(string key, string value, int expDays)
    {
        if (String.IsNullOrEmpty(key)) return;
        await JS.InvokeVoidAsync("setCookie", key, value, expDays);
    }
    public async Task Delete(string key)
    {
        await Set(key, "", -1);
    }
    public async Task DeleteAll()
    {
        var kvps = await GetAll();
        foreach (var kvp in kvps)
        {
            await Set(kvp.Key, "", -1);
        }
    }
}