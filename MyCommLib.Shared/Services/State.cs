using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MyCommLib.Shared.Models.Identity;
using System.Security.Claims;

namespace MyCommLib.Shared.Services;
public class State
{
    //public static string AppTitle = "";
    //public static string AppName = "";
    //public static string AppUrl = "";
    //public static string AppCity = "SYD";
    private NavigationManager NM { get; set; } = default!;
    private HttpClient Http { get; set; } = default!;
    private IJSRuntime JS {  get; set; }
    //private IHostEnvironment env = default!;
    public State(NavigationManager nm, HttpClient http, IJSRuntime js)
    {
        NM = nm;
        //AppUrl = NM.BaseUri;
        Http = http;
        JS = js;
        //this.env = env;
    }
    public CurrentUser CurrentUser { get; set; } = new CurrentUser();
    public string CurrentPage => NM.ToBaseRelativePath(NM.Uri);
    public string GetUri(string path) => $"{path}";
    public bool IsAuthenticated => CurrentUser?.IsAuthenticated ?? false;
    public bool IsInRole(string role)
    {
        foreach (var claim in CurrentUser.Claims.Where(x => x.Type == ClaimTypes.Role))
        {
            if (claim.Value.ToLower() == role.ToLower())
            {
                return true;
            }
        }
        return false;
    }
    public bool IsAdmin => IsInRole("Admins");
    public bool IsAdminUser => (IsInRole("AdminUsers") || IsInRole("Admins"));
    //public bool IsAdminUser => (IsInRole("AdminUsers"));
    public string BaseUri => NM.BaseUri;
    public string EnvironmentName { get; set; } = "";
    public bool IsDevelopment { get; set; } = false;
    public bool IsClockEnabled { get; set; } = true;

    public async Task UpdateEnvironmentClient()
    {
        if (!String.IsNullOrEmpty(this.EnvironmentName)) return;
        try
        {
            var result = await Http.GetAsync("api/admin/EnvName");
            this.EnvironmentName = await result.Content.ReadAsStringAsync();
            result = await Http.GetAsync("api/admin/IsDevelopment");
            var dev = await result.Content.ReadAsStringAsync();
            this.IsDevelopment = (dev == "true");
        }
        catch (Exception ex)
        {
            this.EnvironmentName = ex.Message;
        }
    }
    public string? Username => CurrentUser.Username;
}