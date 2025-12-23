using Blazored.LocalStorage;
using MyCommLib.Shared.Services;
using MyCommLib.Shared.Models.Identity;
using System.Net.Http.Json;

namespace MyCommLib.Client.Services;

public class MyAuthenticationService : IMyAuthenticationService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _ls;
    public MyAuthenticationService(IHttpClientFactory httpFactory, ILocalStorageService ls)
    {
        _http = httpFactory.CreateClient("Api");
        _ls = ls;
    }
    public async Task<LoginResponseModel> LoginUserAsync(LoginModel req)
    {
        var resp = await _http.PostAsJsonAsync("api/auth/login", req);
        if (resp.IsSuccessStatusCode)
        {
            var loginResp = await resp.Content.ReadFromJsonAsync<LoginResponseModel>();
            if (loginResp == null)
            {
                return new LoginResponseModel();
            }
            return loginResp;
        }
        else
        {
            return new LoginResponseModel();
        }
    }

}
