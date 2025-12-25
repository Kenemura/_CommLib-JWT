using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyCommLib.Shared.Models.Identity;
public class LoginModel
{
    [Required]
    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; }
}
