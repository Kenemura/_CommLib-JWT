namespace MyCommLib.Server.Services;

using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MyCommLib.Shared.Interfaces;
public class StateServer : IState
{
    private readonly IConfiguration _config;
    private readonly IHostEnvironment _env;
    public StateServer(IConfiguration config, IHostEnvironment env)
    {
        _config = config;
        _env = env;
    }
    private ClaimsPrincipal currentUser = new(new ClaimsPrincipal());
    public ClaimsPrincipal GetUser() => currentUser;
    public void SetUser(ClaimsPrincipal user)
    {
        if (currentUser != user) currentUser = user;
    }

    public bool IsInRole(string role)
    {
        foreach (var claim in currentUser.Claims.Where(x => x.Type == ClaimTypes.Role))
        {
            if (claim.Value.ToLower() == role.ToLower()) return true;
        }
        return false;
    }
    public bool IsAuthenticated => currentUser.Identity?.IsAuthenticated ?? false;
    public bool IsAdmin => IsInRole("Admins");
    public bool IsAdminUser => IsInRole("Admins") || IsInRole("AdminUsers");

    public async Task<string> GetConfig(string name) => await GetConfig(name, _config);
    public async Task<string> GetConfig(string name, IConfiguration config)
    {
        await Task.CompletedTask;
        for (int i = 0; i < 100; i++)
        {
            var result = config[name];
            //Console.WriteLine($"GetConfig({name}):{result}");
            if (!String.IsNullOrEmpty(result)) return result;
        }
        return "";
    }
    public string EnvironmentName { get; set; } = string.Empty;
    public bool IsDevelopment { get; set; }
    public async Task UpdateState() 
    {
        await Task.CompletedTask;
        EnvironmentName = _env.EnvironmentName;
        IsDevelopment = _env.IsDevelopment();
    }
    public string? Username => currentUser.Identity?.Name;
}
