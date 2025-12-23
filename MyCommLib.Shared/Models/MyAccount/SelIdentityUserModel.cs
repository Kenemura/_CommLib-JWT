using MyCommLib.Components;

namespace MyCommLib.Shared.Models.MyAccount;

public class SelIdentityUserModel
{
    public string? Role { get; set; }
    public MyPagenationModel Paging { get; set; } = new(0, 0);
}
