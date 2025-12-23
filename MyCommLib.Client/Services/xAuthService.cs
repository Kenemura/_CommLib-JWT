using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using MyCommLib.Shared.Models.Identity;
using MyCommLib.Shared.Services;
using System.Security.Claims;

namespace MyCommLib.Client.Services;

public class xAuthService : AuthenticationStateProvider {
    private readonly IdentityApi api;
    private CurrentUser? _currentUser;
    private State State;
    public xAuthService(IdentityApi api, State state) {
        this.api = api;
        State = state;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
        var identity = new ClaimsIdentity();
        try {
            var userInfo = await GetCurrentUser();
            if (userInfo.IsAuthenticated) {
                IEnumerable<Claim> claims = new[] { new Claim(ClaimTypes.Name, _currentUser?.Username ?? "") };
                claims = claims.Concat(_currentUser!.Claims.Select(x => new Claim(x.Type, x.Value)));
                identity = new ClaimsIdentity(claims, "Server authentication");
            }
        } catch (HttpRequestException ex) {
            Console.WriteLine($"Request failed: {ex.ToString()}");
        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
    public async Task<CurrentUser> GetCurrentUser() {
        if (_currentUser is null || !_currentUser.IsAuthenticated) {
            try {
                _currentUser = await api.CurrentUserInfo()!;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                _currentUser = new CurrentUser();
            }
        }
        State.CurrentUser = _currentUser; // Update CurrentUser in State whenever GetCurrentUser is called
        return _currentUser;
    }
    [Authorize]
    public async Task Logout() {
        _currentUser = new CurrentUser();
        await api.Logout();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    public async Task<string> Login(LoginModel req) {
        _currentUser = new CurrentUser();
        try {
            var rmPassword = await api.Login(req);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return rmPassword;
        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }
    public async Task<string> Login2(LoginModel req) {
        _currentUser = new CurrentUser();
        try {
            var rmPassword = await api.Login2(req);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return rmPassword;
        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }
    public async Task<string> AutoLogin(AutoLoginModel req) {
        _currentUser = new CurrentUser();
        try {
            var rmPassword = await api.AutoLogin(req);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return rmPassword;
        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }
    public async Task LoginAs(LoginModel req) {
        _currentUser = new CurrentUser();
        await api.LoginAs(req);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    public async Task<string> LoginWzSC(LoginWzSCModel req) {
        _currentUser = new CurrentUser();
        try {
            var rmPassword = await api.LoginWzSC(req);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return rmPassword;
        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }
}