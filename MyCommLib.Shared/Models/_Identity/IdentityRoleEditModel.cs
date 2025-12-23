namespace MyCommLib.Shared.Models.Identity;

using System.ComponentModel.DataAnnotations;

public class IdentityRoleEditModel
{
    public string? Id { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
}
