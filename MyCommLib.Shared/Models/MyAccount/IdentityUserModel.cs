namespace MyCommLib.Shared.Models.MyAccount;

using System.ComponentModel.DataAnnotations;
public class IdentityUserModel
{
    public string? Id { get; set; } = null!;
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    public string Password { get; set; } = "";
}
