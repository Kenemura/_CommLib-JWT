namespace MyCommLib.Shared.Models.Identity;
public class IdentityUserModel
{
    public string? Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
}
