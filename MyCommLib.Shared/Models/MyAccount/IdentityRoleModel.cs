namespace MyCommLib.Shared.Models.MyAccount;

using System.ComponentModel.DataAnnotations;

public class IdentityRoleModel
{
    public string? Id { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
}
