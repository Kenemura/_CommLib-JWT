namespace MyCommLib.Shared.Models.Identity;

using System.ComponentModel.DataAnnotations;

public class ChangePasswordModel
{
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match!")]
    public string Password2 { get; set; } = null!;
}