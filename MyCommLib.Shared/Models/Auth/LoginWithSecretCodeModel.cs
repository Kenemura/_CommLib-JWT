using System.ComponentModel.DataAnnotations;

namespace MyCommLib.Shared.Models.Identity;
public class LoginWithSecretCodeModel
{
    [Required]
    public string Username { get; set; } = "";
    [Required]
    public string SecretCode { get; set; } = "";
    //public string Email { get; set; } = "";
    public bool RememberMe { get; set; }
}