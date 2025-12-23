namespace MyCommLib.Shared.Models.Identity;

using System.ComponentModel.DataAnnotations;

public class IdentityUserChangePassword
{
    public string Id { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string Password2 { get; set; } = null!;
}
