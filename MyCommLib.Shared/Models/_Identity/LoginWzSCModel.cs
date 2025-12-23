namespace MyCommLib.Shared.Models.Identity;

using System.ComponentModel.DataAnnotations;

public class LoginWzSCModel
{
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string SecretCode { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool RememberMe { get; set; }
}