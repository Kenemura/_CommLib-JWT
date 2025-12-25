using System.Text.Json.Serialization;

namespace MyCommLib.Shared.Models.Auth;

public class LoginResponseModel
{
    [JsonPropertyName("token")]
    public string? AccessToken { get; set; }
    public string? RmPassword { get; set; }
}
