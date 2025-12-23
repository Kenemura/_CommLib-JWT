namespace MyCommLib.Shared.Models.Identity;

using System.ComponentModel.DataAnnotations;

public class CreateRequestModel
{
    [Required]
    public string Username { get; set; } = "";
    [EmailAddress]
    public string Email { get; set; } = "";
}