using System.ComponentModel.DataAnnotations;

namespace MyCommLib.Shared.Models.Auth;
public class LoginRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";
    public string? AppTitle { get; set; }
    public string? AppName { get; set; }
    public string? AppUrl { get; set; }
}