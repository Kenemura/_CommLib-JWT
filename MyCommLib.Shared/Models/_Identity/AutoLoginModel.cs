namespace MyCommLib.Shared.Models.Identity;

using System.ComponentModel.DataAnnotations;

public class AutoLoginModel
{
    public string Username { get; set; } = "";
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";
    public bool RememberMe { get; set; }
    public string UserId { get; set; } = "";
}