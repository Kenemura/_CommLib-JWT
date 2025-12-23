using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Identity;
using MyCommLib.Shared.Interfaces;
using MyCommLib.Shared.Models.MyAccount;

namespace MyCommLib.Server.Services;

public class IdentityApiServer : IIdentityApi
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public IdentityApiServer(UserManager<IdentityUser> um, SignInManager<IdentityUser> sm, RoleManager<IdentityRole> rm)
    {
        _userManager = um;
        _signInManager = sm;
        _roleManager = rm;
    }
    public async Task<int> GetUsersCount(SelIdentityUserModel sel)
    {
        var items = await getUsers(sel);
        return items.Count();
    }
    public async Task<List<IdentityUserModel>> GetUsers(SelIdentityUserModel sel)
    {
        var items = (await getUsers(sel)).OrderBy(x => x.UserName);
        if (sel.Paging!.CurrentPage > 0)
        {
            items = items.Skip((sel.Paging!.CurrentPage - 1) * sel.Paging.PageSize).Take(sel.Paging.PageSize).OrderBy(x => x.UserName);
        }
        var items2 = new List<IdentityUserModel>();
        foreach (var item in items)
        {
            items2.Add(new IdentityUserModel() { Id = item.Id, Username = item.UserName! });
        }
        return items2;
    }
    private async Task<IEnumerable<IdentityUser>> getUsers(SelIdentityUserModel sel) =>
        (sel.Role == "-") ? _userManager.Users :
        (await _userManager.GetUsersInRoleAsync(sel.Role!));

    public async Task<int> GetRolesCount()
    {
        await Task.CompletedTask;
        return _roleManager.Roles.Count();
    }
    public async Task<List<IdentityRoleModel>> GetRoles()
    {
        var sel = new SelIdentityRoleModel();
        return await getRoles(sel);
    }
    public async Task<List<IdentityRoleModel>> GetRoles(SelIdentityRoleModel sel)
    {
        return await getRoles(sel);
    }
    private async Task<List<IdentityRoleModel>> getRoles(SelIdentityRoleModel sel)
    {
        await Task.CompletedTask;
        var items = _roleManager.Roles.OrderBy(x => x.Name).ToList();
        if (sel.Paging!.CurrentPage > 0)
        {
            items = items.Skip((sel.Paging.CurrentPage - 1) * sel.Paging.PageSize).Take(sel.Paging.PageSize).ToList();
        }
        var items2 = new List<IdentityRoleModel>();
        foreach (var item in items)
        {
            items2.Add(new IdentityRoleModel() { Id = item.Id, Name = item.Name! });
        }
        return items2;
    }
    public Task<List<IdentityRoleModel>> GetRoles(int pageSize, int pageNo)
    {
        throw new NotImplementedException();
    }
    public async Task<IdentityUserModel> GetUser(string id)
    {
        var u = await _userManager.FindByIdAsync(id) ?? new();
        return new IdentityUserModel() { Id = u.Id, Username = u.UserName!, Email = u.Email! };
    }
    public async Task CreateUser(IdentityUserModel userEdit)
    {
        var user = new IdentityUser();
        user.UserName = userEdit.Username;
        user.Email = userEdit.Email;
        var result = await _userManager.CreateAsync(user, userEdit.Password);
        if (!result.Succeeded) throw new Exception(result.Errors.FirstOrDefault()?.Description);
    }
    public async Task UpdateUser(IdentityUserModel userEdit)
    {
        var user = await _userManager.FindByIdAsync(userEdit?.Id ?? "");
        user!.Email = userEdit?.Email;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded && !String.IsNullOrEmpty(userEdit?.Password))
        {
            await _userManager.RemovePasswordAsync(user);
            result = await _userManager.AddPasswordAsync(user, userEdit.Password);
        }
        if (!result.Succeeded) throw new Exception(result.Errors.FirstOrDefault()?.Description);
    }
    public async Task DeleteUser(IdentityUserModel userEdit)
    {
        var user = await _userManager.FindByIdAsync(userEdit?.Id ?? "");
        if (user is null) throw new Exception("Not found!");
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded) throw new Exception(result.Errors.FirstOrDefault()?.Description);
    }
    public async Task<IdentityRoleModel> GetRole(string id)
    {
        var r = await _roleManager.FindByIdAsync(id) ?? new();
        return new IdentityRoleModel() { Id = r.Id, Name = r.Name! };
    }
    public async Task CreateRole(IdentityRoleModel roleEdit)
    {
        var role = new IdentityRole();
        role.Name = roleEdit.Name;
        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded) throw new Exception(result.Errors.FirstOrDefault()?.Description);
    }
    public async Task DeleteRole(IdentityRoleModel roleEdit)
    {
        var role = await _roleManager.FindByIdAsync(roleEdit?.Id ?? "");
        if (role is null) throw new Exception("Not found!");
        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded) throw new Exception(result.Errors.FirstOrDefault()?.Description);
    }
    public async Task<bool> IsUserInRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var result = await _userManager.IsInRoleAsync(user!, role);
        return result;
    }
    public async Task SetUserRole(IdentityUserRoleModel ur)
    {
        var user = await _userManager.FindByIdAsync(ur.Id);
        IdentityResult? result;
        if (ur.IsInRole)
        {
            result = await _userManager.AddToRoleAsync(user!, ur.Role);
        }
        else
        {
            result = await _userManager.RemoveFromRoleAsync(user!, ur.Role);
        }
        if (!result.Succeeded) throw new Exception(result.Errors.FirstOrDefault()?.Description);
    }
}
