using MyCommLib.Shared.Models.Identity;

namespace MyCommLib.Client.Services;

public interface IMyAuthenticationService
{
    Task<LoginResponseModel> LoginUserAsync(LoginModel req);
}
