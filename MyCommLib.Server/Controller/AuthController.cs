using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyCommLib.Server.Classes;
using MyCommLib.Shared.Models.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyCommLib.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly  IConfiguration _cfg;
    public AuthController(IConfiguration cfg)
    {
        _cfg = cfg;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginModel req)
    {
        if (!(req.Username == "user1" && req.Password == "41Rocklands")
            && !(req.Username == "user2" && req.Password == "41Rocklands"))
        {
            return Unauthorized();
        }

        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, req.Username),
            new(ClaimTypes.Name, req.Username),
            new(ClaimTypes.Role, req.Username == "user1" ? "Admin" : "User")
        };

        var secretBytes = Convert.FromBase64String(_cfg["Jwt:Key"]!);
        var key = new SymmetricSecurityKey(secretBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:AccessTokenMinutes"]!)),
            signingCredentials: creds);

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

        var resp = new LoginResponseModel();
        resp.AccessToken = tokenStr;
        if (req.RememberMe)
        {
            var rmPassword = clsRememberMe.GetEncrypted(req.Password);
            resp.RmPassword = rmPassword;
        }
        return Ok(resp);
    }
}
