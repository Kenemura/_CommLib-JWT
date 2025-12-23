namespace MyCommLib.Shared.Models.MyAccount;

using System.ComponentModel.DataAnnotations;
public class IdentityUserRoleModel
{
    public string Id { get; set; } = null!;
    public string Role { get; set; } = null!;
    public bool IsInRole { get; set; }
}
