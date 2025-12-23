using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MyCommLib.Shared.Models.Identity;
using MyCommLib.Shared.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyCommLib.Client.Services;

public class MyJwtASP : AuthenticationStateProvider
{
    private readonly ILocalStorageService _ls;
    private State _state;

    public MyJwtASP(ILocalStorageService ls, State state)
    {
        _ls = ls;
        _state = state;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (await _ls.ContainKeyAsync("access_token"))
        {
            var tokenStr = await _ls.GetItemAsync<string>("access_token");
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenStr);
            var identity = new ClaimsIdentity(token.Claims, "Jwt");
            if (token.ValidTo < DateTime.UtcNow)
            {
                return SetAnonymousState();
            }
            return SetAuthenticatedState(identity);
        }
        return SetAnonymousState();
    }
    protected AuthenticationState SetAuthenticatedState(ClaimsIdentity identity)
    {
        var user = new ClaimsPrincipal(identity);
        var authState = new AuthenticationState(user);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
        _state.CurrentUser = GetCurrentUser(identity); // Update CurrentUser in State whenever GetAuthenticationStateAsync is called
        return authState;
    }

    protected AuthenticationState SetAnonymousState()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var anonymousAuthState = new AuthenticationState(anonymousUser);
        NotifyAuthenticationStateChanged(Task.FromResult(anonymousAuthState));
        _state.CurrentUser = new CurrentUser(); // Update CurrentUser in State whenever GetAuthenticationStateAsync is called
        return anonymousAuthState;
    }

    private CurrentUser GetCurrentUser(ClaimsIdentity identity)
    {
        var user = new CurrentUser()
        {
            IsAuthenticated = identity.IsAuthenticated,
            Username = identity?.Name ?? "",
            Claims = identity?.Claims.Select(x => new CurrentUserClaim() { Type = x.Type, Value = x.Value }).ToList()!
        };
        return user;
    }

}
