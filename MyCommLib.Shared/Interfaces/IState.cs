namespace MyCommLib.Shared.Interfaces;

using System.Security.Claims;
public interface IState
{
    public ClaimsPrincipal GetUser();
    public void SetUser(ClaimsPrincipal user);
    public bool IsInRole(string role);
    public bool IsAuthenticated { get; }
    public bool IsAdmin { get; }
    public bool IsAdminUser { get; }

    public Task<string> GetConfig(string name);
    public string EnvironmentName { get; set; }
    public bool IsDevelopment { get; set; }

    public Task UpdateState();

    public string? Username { get; }
}
