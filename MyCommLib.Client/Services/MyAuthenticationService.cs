using Blazored.LocalStorage;
using MyCommLib.Shared.Models.Auth;
using MyCommLib.Shared.Models.Identity;
using MyCommLib.Shared.Services;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

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
        var loginResp = new LoginResponseModel();
        var resp = await _http.PostAsJsonAsync("api/auth/login", req);
        if (resp.IsSuccessStatusCode)
        {
            loginResp = await resp.Content.ReadFromJsonAsync<LoginResponseModel>();
            if (loginResp == null) loginResp = new LoginResponseModel();
        }
        return loginResp;
    }
    public async Task<LoginResponseModel> LoginUserAsync(LoginWithSecretCodeModel req)
    {
        var loginResp = new LoginResponseModel();
        var resp = await _http.PostAsJsonAsync("api/auth/LoginWithSecretCode", req);
        if (resp.IsSuccessStatusCode)
        {
            loginResp = await resp.Content.ReadFromJsonAsync<LoginResponseModel>();
            if (loginResp == null) loginResp = new LoginResponseModel();
        }
        return loginResp;
    }
    public async Task LoginRequest(LoginRequestModel req)
    {
        var result = await _http.PostAsJsonAsync($"api/auth/LoginRequest", req);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }

}
