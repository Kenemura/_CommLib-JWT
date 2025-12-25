using System.ComponentModel.DataAnnotations;

namespace MyCommLib.Shared.Models.Identity;
public class LoginWithSecretCodeModel
{
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string SecretCode { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool RememberMe { get; set; }
}