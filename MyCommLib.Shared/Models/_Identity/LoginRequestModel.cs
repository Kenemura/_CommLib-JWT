namespace MyCommLib.Shared.Models.Identity;

using System.ComponentModel.DataAnnotations;

public class LoginRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";
}