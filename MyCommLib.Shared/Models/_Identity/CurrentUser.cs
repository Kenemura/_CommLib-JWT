namespace MyCommLib.Shared.Models.Identity;
public class CurrentUser
{
    public bool IsAuthenticated { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public List<CurrentUserClaim> Claims { get; set; } = new List<CurrentUserClaim>();
}
public class CurrentUserClaim
{
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
}