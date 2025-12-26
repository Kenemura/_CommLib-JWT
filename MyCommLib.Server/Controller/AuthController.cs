using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyCommLib.Classes;
using MyCommLib.Server.Classes;
using MyCommLib.Server.Services;
using MyCommLib.Shared.Models.Auth;
using MyCommLib.Shared.Models.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyCommLib.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly  IConfiguration _cfg;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly MyEmailSender _emailSender;
    public AuthController(IConfiguration cfg, UserManager<IdentityUser> um, SignInManager<IdentityUser> sm, RoleManager<IdentityRole> rm, MyEmailSender ems)
    {
        _cfg = cfg;
        _userManager = um;
        _signInManager = sm;
        _roleManager = rm;
        _emailSender = ems;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel req)
    {
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user == null) return BadRequest("User does not exist");
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!signInResult.Succeeded) return BadRequest("Invalid password");

        var resp = new LoginResponseModel();
        resp.AccessToken = GetJwtToken(user);
        if (req.RememberMe)
        {
            var rmPassword = clsRememberMe.GetEncrypted(req.Password);
            var test = clsRememberMe.GetDecrypted(rmPassword);
            resp.RmPassword = rmPassword;
        }
        Console.WriteLine($"Login OK!");
        return Ok(resp);
    }

    private string GetJwtToken(IdentityUser? user)
    {
        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, user!.UserName!),
            new(ClaimTypes.Name, user!.UserName!),
            new(ClaimTypes.Email, user.Email ?? ""),
        };
        var roles = _roleManager.Roles.Select(x => new IdentityRoleModel() { Id = x.Id, Name = x.Name! }).ToList();
        foreach (var role in roles)
        {
            claims.Add(new(ClaimTypes.Role, role.Name));
        }

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
        return tokenStr;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWzRememberMe([FromBody] LoginModel req)
    {
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user == null) return BadRequest("User does not exist");
        try
        {
            var pwd = clsRememberMe.GetDecrypted(req.Password);
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, pwd, false);
            if (!signInResult.Succeeded) return BadRequest("Invalid password");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        var resp = new LoginResponseModel();
        resp.AccessToken = GetJwtToken(user);
        if (req.RememberMe)
        {
            var rmPassword = req.Password;
            resp.RmPassword = rmPassword;
        }
        Console.WriteLine($"LoginWzRememberMe OK!");
        return Ok(resp);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWzSecretCode(LoginWzSecretCodeModel req)
    {
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user == null) return BadRequest("User does not exist");
        var password = clsAccountHash.GetHashedPassword(user!.Email!);
        if (IsLoginWzSCOk(req.SecretCode, user!.Email!))
        {
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, password);

            var result = await _signInManager.PasswordSignInAsync(req.Username, password, false, false);
            if (!result.Succeeded)
            {
                throw new Exception("Unexpected error");
            }
        }
        else
        {
            return BadRequest("Incorrect UserId");
        }

        var resp = new LoginResponseModel();
        resp.AccessToken = GetJwtToken(user);
        if (req.RememberMe)
        {
            var rmPassword = clsRememberMe.GetEncrypted(password);
            resp.RmPassword = rmPassword;
        }
        Console.WriteLine($"LoginWzSecretCode OK!");
        return Ok(resp);
    }
    private bool IsLoginWzSCOk(string secretCode, string email)
    {
        if (secretCode == clsAccountHash.GetSecretCode(email, DateTime.Now)) return true;
        if (secretCode == clsAccountHash.GetSecretCode(email, DateTime.Now.AddDays(-1))) return true;
        return false;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> LoginRequest(LoginRequestModel req)
    {
        await Task.CompletedTask;
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null) return BadRequest("User does not exist");
        _emailSender.AddTo(user.UserName!, user.Email!);
        _emailSender.AddSubject($"Your Login Info for {req.AppTitle}");
        _emailSender.AddHtmlBody(EmailBodyLoginReq(user, req));
        _emailSender.AddFromName(req.AppName ?? "");
        var result = await _emailSender.Send();
        if (!String.IsNullOrEmpty(result)) return BadRequest(result);
        return Ok();
    }
    private string EmailBodyLoginReq(IdentityUser user, LoginRequestModel req)
    {
        string body = $"<h3>Hello {user.UserName}-san!</h3>";
        body += $"<p>Use your User Name and Secret Code to Login.</p>";
        body += $"<p> - User Name: {user.UserName}";
        body += $"<br/> - Secret Code: {clsAccountHash.GetSecretCode(user.Email!, DateTime.Now)}";
        body += "<br/>(This Secret Code is valid only upto 24 hours for security reason)</p>";
        body += EmailFooter(req);
        body = "<font size=\"+1\">" + body + "</font>";
        return body;
    }
    private string EmailFooter(LoginRequestModel req)
    {
        var ft = "<br/>";
        ft += "==================================================<br/>";
        ft += $"{req.AppName}<br/>";
        ft += $"URL: {req.AppUrl}<br/>";
        ft += "==================================================<br/>";
        return ft;
    }
}
