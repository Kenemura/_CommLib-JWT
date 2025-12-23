using Microsoft.AspNetCore.Identity;
using MyCommLib.Shared.Models.MyAccount;

namespace MyCommLib.Shared.Interfaces;
public interface IIdentityApi
{
    public Task<int> GetUsersCount(SelIdentityUserModel sel);
    public Task<List<IdentityUserModel>> GetUsers(SelIdentityUserModel sel);
    public Task<List<IdentityRoleModel>> GetRoles();
    public Task<int> GetRolesCount();
    public Task<List<IdentityRoleModel>> GetRoles(SelIdentityRoleModel sel);
    public Task<IdentityUserModel> GetUser(string id);
    public Task UpdateUser(IdentityUserModel userEdit);
    public Task CreateUser(IdentityUserModel userEdit);
    public Task DeleteUser(IdentityUserModel userEdit);
    public Task<IdentityRoleModel> GetRole(string id);
    public Task CreateRole(IdentityRoleModel roleEdit);
    public Task DeleteRole(IdentityRoleModel roleEdit);
    public Task<bool> IsUserInRole(string userId, string role);
    public Task SetUserRole(IdentityUserRoleModel ur);
}