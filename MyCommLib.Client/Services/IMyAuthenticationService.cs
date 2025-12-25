using MyCommLib.Shared.Models.Auth;
using MyCommLib.Shared.Models.Identity;

namespace MyCommLib.Client.Services;

public interface IMyAuthenticationService
{
    Task<LoginResponseModel> LoginUserAsync(LoginModel req);
    Task<LoginResponseModel> LoginUserAsync(LoginWithSecretCodeModel req);
    Task LoginRequest(LoginRequestModel req);
}
