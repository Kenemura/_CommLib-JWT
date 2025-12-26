using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyCommLib.Shared.Models.Identity;
public class LoginModel
{
    [Required]
    [JsonPropertyName("username")]
    public string Username { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
}
