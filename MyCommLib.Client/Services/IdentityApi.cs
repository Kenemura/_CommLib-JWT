namespace MyCommLib.Client.Services;

using Microsoft.AspNetCore.Identity;
using MyCommLib.Shared.Models.Identity;
using System.Net.Http.Json;

public class IdentityApi
{
    private readonly HttpClient Http;
    public IdentityApi (HttpClient hc)
    {
        Http = hc;
    }
    public async Task<CurrentUser> CurrentUserInfo()
    {
        try
        {
            var result = await Http.GetFromJsonAsync<CurrentUser>("api/identity/currentuserinfo");
            return result!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new CurrentUser();
        }
    }
    public async Task<string> Login(LoginModel req)
    {
        var result = await Http.PostAsJsonAsync("api/identity/login", req);
        if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
        return await result.Content.ReadAsStringAsync();
    }
    public async Task<string> Login2(LoginModel req)
    {
        var result = await Http.PostAsJsonAsync("api/identity/login2", req);
        if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
        return await result.Content.ReadAsStringAsync();
    }
    public async Task<string> AutoLogin(AutoLoginModel req)
    {
        var result = await Http.PostAsJsonAsync($"api/identity/autologin", req);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
        return await result.Content.ReadAsStringAsync();
    }
    public async Task LoginAs(LoginModel req)
    {
        var result = await Http.PostAsJsonAsync("api/identity/loginas", req);
        if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task<string> LoginWzSC(LoginWzSCModel req) {
        var result = await Http.PostAsJsonAsync("api/identity/LoginWzSC", req);
        if (result.StatusCode == System.Net.HttpStatusCode.BadRequest) {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
        return await result.Content.ReadAsStringAsync();
    }
    public async Task Logout()
    {
        var result = await Http.PostAsync("api/identity/logout", null);
        result.EnsureSuccessStatusCode();
    }
    public async Task<int> GetUsersCount(string role)
    {
        var route = $"api/identity/getuserscount/{role}";
        return await Http.GetFromJsonAsync<int>(route);
    }
    public async Task<List<IdentityUserModel>> GetUsers(string role, int pageSize, int pageNo)
    {
        var route = $"api/identity/getusers/{role}/{pageSize}/{pageNo}";
        var result =  await Http.GetFromJsonAsync<List<IdentityUserModel>>(route);
        return result ?? new List<IdentityUserModel>();
    }
    public async Task<List<IdentityRoleModel>> GetRoles()
    {
        var result = await Http.GetFromJsonAsync<IEnumerable<IdentityRoleModel>>("api/identity/getroles");
        return result?.ToList() ?? new List<IdentityRoleModel>();
    }
    public async Task<int> GetRoleCount()
    {
        var route = $"api/identity/getrolecount";
        return await Http.GetFromJsonAsync<int>(route);
    }
    public async Task<List<IdentityRoleModel>> GetRoles(int pageSize, int pageNo)
    {
        var route = $"api/identity/getroles/{pageSize}/{pageNo}";
        var result = await Http.GetFromJsonAsync<List<IdentityRoleModel>>(route);
        return result ?? new List<IdentityRoleModel>();
    }
    public async Task<IdentityUserModel> GetUser(string id)
    {
        var resp = await Http.GetAsync($"api/identity/getuserById/{id}");
        var st = await resp.Content.ReadAsStringAsync(); // for testing
        try
        {
            var result = await resp.Content.ReadFromJsonAsync<IdentityUserModel>();
            return result!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex.Message}");
        }
        return null!;
    }
    public async Task UpdateUser(IdentityUserEditModel userEdit)
    {
        var result = await Http.PostAsJsonAsync("api/identity/updateUser", userEdit);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task CreateUser(IdentityUserEditModel userEdit)
    {
        var result = await Http.PostAsJsonAsync("api/identity/createUser", userEdit);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task CreateUser2(IdentityUserEditModel userEdit) {
        var result = await Http.PostAsJsonAsync("api/identity/createUser2", userEdit);
        if (result.StatusCode != System.Net.HttpStatusCode.OK) {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task DeleteUser(IdentityUserEditModel userEdit)
    {
        var result = await Http.PostAsJsonAsync($"api/identity/deleteuser", userEdit);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task<IdentityRoleModel> GetRole(string id)
    {
        var resp = await Http.GetAsync($"api/identity/getroleById/{id}");
        var st = await resp.Content.ReadAsStringAsync(); // for testing
        try
        {
            var result = await resp.Content.ReadFromJsonAsync<IdentityRoleModel>();
            return result!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex.Message}");
        }
        return null!;
    }
    public async Task CreateRole(IdentityRoleEditModel roleEdit)
    {
        var result = await Http.PostAsJsonAsync("api/identity/createRole", roleEdit);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task DeleteRole(IdentityRoleEditModel roleEdit)
    {
        var result = await Http.PostAsJsonAsync($"api/identity/deleteRole", roleEdit);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task<bool> UserIsInRole(string userId, string role)
    {
        var result = await Http.GetFromJsonAsync<bool>($"api/identity/IsUserInRole/{userId}/{role}");
        return result;
    }
    public async Task SetUserRole(IdentityUserRoleModel ur)
    {
        var result = await Http.PostAsJsonAsync($"api/identity/SetUserRole", ur);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task CreateRequest(CreateRequestModel req)
    {
        var result = await Http.PostAsJsonAsync($"api/identity/CreateRequest", req);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task CreateRequest2(CreateRequestModel req) {
        var result = await Http.PostAsJsonAsync($"api/identity/CreateRequest2", req);
        if (result.StatusCode != System.Net.HttpStatusCode.OK) {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task LoginRequest(LoginRequestModel req)
    {
        var result = await Http.PostAsJsonAsync($"api/identity/LoginRequest", req);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task LoginRequest2(LoginRequestModel req) {
        var result = await Http.PostAsJsonAsync($"api/identity/LoginRequest2", req);
        if (result.StatusCode != System.Net.HttpStatusCode.OK) {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task ChangePassword(ChangePasswordModel req)
    {
        var result = await Http.PostAsJsonAsync($"api/identity/changepassword", req);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
}