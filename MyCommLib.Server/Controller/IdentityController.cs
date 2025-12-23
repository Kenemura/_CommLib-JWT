namespace MyCommLib.Server.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCommLib.Classes;
using MyCommLib.Server.Classes;
using MyCommLib.Server.Services;
using MyCommLib.Shared.Services;
using MyCommLib.Shared.Models.Identity;
//using MyCommLib.Shared.Services;
using System.Security.Claims;

[Route("api/[controller]/[action]")]
[ApiController]
public class IdentityController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly MyEmailSender _emailSender;
    public IdentityController(UserManager<IdentityUser> um, SignInManager<IdentityUser> sm,
        RoleManager<IdentityRole> rm, MyEmailSender ems)
    {
        _userManager = um;
        _signInManager = sm;
        _roleManager = rm;
        _emailSender = ems;
    }

    [HttpGet]
    public CurrentUser CurrentUserInfo()
    {
        var currentUser = new CurrentUser
        {
            IsAuthenticated = (User.Identity?.IsAuthenticated ?? false),
            Username = User.Identity?.Name ?? "",
            Email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? "",
            Claims = User.Claims.Select(x => new CurrentUserClaim() { Type = x.Type, Value = x.Value }).ToList()
        };
        return currentUser;
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginModel req)
    {
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user == null) return BadRequest("User does not exist");
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!signInResult.Succeeded) return BadRequest("Invalid password");
        //await _signInManager.SignInAsync(user, req.RememberMe);
        await _signInManager.SignInAsync(user, false); // isPersistent = false
        if (req.RememberMe)
        {
            var rmPassword = clsRememberMe.GetEncrypted(req.Password);
            return Ok(rmPassword);
        }
        return Ok("");
    }
    [HttpPost]
    public async Task<IActionResult> Login2(LoginModel req)
    {
        var pwd = clsRememberMe.GetDecrypted(req.Password);
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user == null) return BadRequest("User does not exist");
        //pwd = "";
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, pwd, false);
        if (!signInResult.Succeeded) return BadRequest("Invalid password");
        //await _signInManager.SignInAsync(user, req.RememberMe);
        await _signInManager.SignInAsync(user, false); // not to use cookie
        var rmPassword = clsRememberMe.GetEncrypted(pwd);
        return Ok(rmPassword);
    }
    [HttpPost]
    public async Task<IActionResult> AutoLogin(AutoLoginModel req)
    {
        var password = clsAccountHash.GetHashedPassword(req.Email);
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user == null) return BadRequest("User does not exist");
        if (IsAutoLoginUserIdOk(req))
        {
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, password);

            //var result = await _signInManager.PasswordSignInAsync(req.UserName, password, req.RememberMe, false);
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
        if (req.RememberMe)
        {
            var rmPassword = clsRememberMe.GetEncrypted(password);
            return Ok(rmPassword);
        }
        return Ok("");
    }
    private bool IsAutoLoginUserIdOk(AutoLoginModel req)
    {
        if (req.UserId == clsAccountHash.GetHashedUserId(req.Email, DateTime.Now)) return true;
        if (req.UserId == clsAccountHash.GetHashedUserId(req.Email, DateTime.Now.AddDays(-1))) return true;
        return false;
    }
    [HttpPost]
    public async Task<IActionResult> LoginWzSC(LoginWzSCModel req) {
        var password = clsAccountHash.GetHashedPassword(req.Email);
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user == null) return BadRequest("User does not exist");
        if (IsLoginWzSCOk(req)) {
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, password);

            var result = await _signInManager.PasswordSignInAsync(req.Username, password, false, false);
            if (!result.Succeeded) {
                throw new Exception("Unexpected error");
            }
        } else {
            return BadRequest("Incorrect UserId");
        }
        if (req.RememberMe) {
            var rmPassword = clsRememberMe.GetEncrypted(password);
            return Ok(rmPassword);
        }
        return Ok("");
    }
    private bool IsLoginWzSCOk(LoginWzSCModel req) {
        if (req.SecretCode == clsAccountHash.GetSecretCode(req.Email, DateTime.Now)) return true;
        if (req.SecretCode == clsAccountHash.GetSecretCode(req.Email, DateTime.Now.AddDays(-1))) return true;
        return false;
    }
    [Authorize(Roles = "Admins, AdminUsers")]
    [HttpPost]
    public async Task<IActionResult> LoginAs(LoginModel req)
    {
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user == null) return BadRequest("User does not exist");
        await _signInManager.SignOutAsync();
        //await _signInManager.SignInAsync(user, req.RememberMe);
        await _signInManager.SignInAsync(user, false);
        return Ok();
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpGet("{role}")]
    public async Task<int> GetUsersCount(string? role) => (await getUsers(role)).Count();

    [HttpGet("{role}/{pagesize:int}/{pageno:int}")]
    public async Task<IEnumerable<IdentityUserModel>> GetUsers(string? role, int pageSize, int pageNo) =>
        (await getUsers(role)).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

    private async Task<IEnumerable<IdentityUserModel>> getUsers(string? role)
    {
        IEnumerable<IdentityUser> users = _userManager.Users.OrderBy(x => x.UserName);
        if (role != "-") users = await _userManager.GetUsersInRoleAsync(role ?? "");
        return users.Select(x => UserModel(x)).OrderBy(x => x.Username);
    }
    private IdentityUserModel UserModel(IdentityUser user)
        => new IdentityUserModel() { Id = user.Id, Username = user.UserName!, Email = user.Email! };

    [HttpGet]
    public async Task<int> GetRoleCount()
    {
        await Task.CompletedTask;
        return _roleManager.Roles.Count();
    }
    [HttpGet]
    public async Task<IEnumerable<IdentityRoleModel>> GetRoles()
    {
        await Task.CompletedTask;
        return _roleManager.Roles.Select(x => new IdentityRoleModel() { Id = x.Id, Name = x.Name! }).ToList();
    }
    [HttpGet("{pageSize:int}/{pageNo:int}")]
    public async Task<IEnumerable<IdentityRoleModel>> GetRoles(int pageSize, int pageNo)
    {
        await Task.CompletedTask;
        //var roles = _roleManager.Roles;
        var roles = _roleManager.Roles.Select(x => new IdentityRoleModel() { Id = x.Id, Name = x.Name! });
        return roles.Skip((pageNo - 1) * pageSize).Take(pageSize).OrderBy(x => x.Name).ToList();
    }
    //private IdentityRoleModel RoleModel(IdentityRole role)
    //    => new IdentityRoleModel() { Id = role.Id, Name = role.Name! };

    [HttpGet("{id}")]
    public async Task<IdentityUser> GetUserById(string id)
    {
        return await _userManager.FindByIdAsync(id) ?? new IdentityUser();
    }
    [HttpPost]
    public async Task<IActionResult> UpdateUser(IdentityUserEditModel userEdit)
    {
        var user = await _userManager.FindByIdAsync(userEdit?.Id ?? "");
        user!.Email = userEdit?.Email;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded && !String.IsNullOrEmpty(userEdit?.Password))
        {
            await _userManager.RemovePasswordAsync(user);
            result = await _userManager.AddPasswordAsync(user, userEdit.Password);
        }
        if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> CreateUser(IdentityUserEditModel userEdit)
    {
        var user = new IdentityUser();
        user.UserName = userEdit.Username;
        user.Email = userEdit.Email;
        var result = await _userManager.CreateAsync(user, userEdit.Password);
        if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> DeleteUser(IdentityUserEditModel userEdit)
    {
        var user = await _userManager.FindByIdAsync(userEdit!.Id!);
        if (user is null) return BadRequest("Not found!");
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
        return Ok();
    }
    [HttpGet("{id}")]
    public async Task<IdentityRoleModel> GetRoleById(string id)
    {
        var role = await _roleManager.FindByIdAsync(id) ?? new IdentityRole();
        return new IdentityRoleModel() { Id = role.Id, Name = role.Name! };
    }
    [HttpPost]
    public async Task<IActionResult> CreateRole(IdentityRoleEditModel roleEdit)
    {
        var role = new IdentityRole();
        role.Name = roleEdit.Name;
        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> DeleteRole(IdentityRoleEditModel roleEdit)
    {
        var role = await _roleManager.FindByIdAsync(roleEdit!.Id!);
        if (role is null) return BadRequest("Not found!");
        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
        return Ok();
    }
    [HttpGet("{userId}/{role}")]
    public async Task<bool> IsUserInRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var result = await _userManager.IsInRoleAsync(user!, role);
        return result;
    }
    [HttpPost]
    public async Task<IActionResult> SetUserRole(IdentityUserRoleModel ur)
    {
        var user = await _userManager.FindByIdAsync(ur.Id);
        IdentityResult? result;
        if (ur.IsInRole)
        {
            result = await _userManager.AddToRoleAsync(user!, ur.Role);
        }
        else
        {
            result = await _userManager.RemoveFromRoleAsync(user!, ur.Role);
        }
        if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> LoginRequest(AutoLoginModel req)
    {
        await Task.CompletedTask;
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null) return BadRequest("User does not exist");
        //State.AppUrl = clsHttpContext.GetBaseUrl(HttpContext); // Set BaseUrl in AppInfo
        //var result = await AutoLoginEmail.Send(user);
        _emailSender.AddTo(user.UserName!, user.Email!);
        _emailSender.AddSubject($"Auto Login to {State.AppTitle}");
        _emailSender.AddHtmlBody(EmailBodyLoginReq(user));
        var result = await _emailSender.Send();
        if (!String.IsNullOrEmpty(result)) return BadRequest(result);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> LoginRequest2(AutoLoginModel req) {
        await Task.CompletedTask;
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null) return BadRequest("User does not exist");
        _emailSender.AddTo(user.UserName!, user.Email!);
        _emailSender.AddSubject($"Your Login Info for {State.AppTitle}");
        _emailSender.AddHtmlBody(EmailBodyLoginReq2(user));
        var result = await _emailSender.Send();
        if (!String.IsNullOrEmpty(result)) return BadRequest(result);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> CreateRequest(CreateRequestModel req)
    {
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user is not null) return BadRequest($"User Name is already taken. Please choose different one!");
        user = await _userManager.FindByEmailAsync(req.Email);
        if (user is not null) return BadRequest("An account with taht email exists. If it is yours, use Login Request!");
        user = new IdentityUser { UserName = req.Username, Email = req.Email };
        string pwd = clsAccountHash.GetHashedPassword(req.Email);
        var result = await _userManager.CreateAsync(user, pwd);
        if (!result.Succeeded) throw new Exception(result.Errors.FirstOrDefault()?.Description);

        //var result2 = await AutoLoginEmail.SendCreated(user);
        _emailSender.AddTo(user.UserName!, user.Email!);
        _emailSender.AddSubject($"Auto Login to {State.AppTitle}");
        _emailSender.AddHtmlBody(EmailBodyCreateReq(user));
        var result2 = await _emailSender.Send();
        if (!String.IsNullOrEmpty(result2)) return BadRequest(result);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> CreateRequest2(CreateRequestModel req) {
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user is not null) return BadRequest($"User Name is already taken. Please choose different one!");
        user = await _userManager.FindByEmailAsync(req.Email);
        if (user is not null) return BadRequest("An account with taht email exists. If it is yours, use Login Request!");
        user = new IdentityUser { UserName = req.Username, Email = req.Email };
        string pwd = clsAccountHash.GetHashedPassword(req.Email);
        var result = await _userManager.CreateAsync(user, pwd);
        if (!result.Succeeded) throw new Exception(result.Errors.FirstOrDefault()?.Description);
        return Ok();
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel req)
    {
        var user = await _userManager.FindByNameAsync(User?.Identity?.Name ?? "");
        if (user == null) return BadRequest("User does not exist");
        var result = await _userManager.RemovePasswordAsync(user);
        if (result.Succeeded)
        {
            result = await _userManager.AddPasswordAsync(user, req.Password);
        }
        if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
        return Ok();
    }

    private string EmailBodyLoginReq(IdentityUser user)
    {
        string url = $"{State.AppUrl.ToLower()}Identity/AutoLogin";
        url += $"?UserName={user.UserName}&Email={user.Email}";
        url += $"&UserId={clsAccountHash.GetHashedUserId(user.Email!, DateTime.Now)}";

        string body = $"<h3>Hello {user.UserName}-san!</h3>";
        body += $@"<p>You can login to the site by using this <a href=""{url}"">Link</a>.";
        body += "<br/>(The above link works only upto 24 hours for security reason)</p>";
        body += EmailFooter();
        body = "<font size=\"+1\">" + body + "</font>";
        return body;
    }
    private string EmailBodyLoginReq2(IdentityUser user) {
        string body = $"<h3>Hello {user.UserName}-san!</h3>";
        body += $"<p>Use your User Name and Secret Code to Login.</p>";
        body += $"<p> - User Name: {user.UserName}";
        body += $"<br/> - Secret Code: {clsAccountHash.GetSecretCode(user.Email!, DateTime.Now)}";
        body += "<br/>(This Secret Code is valid only upto 24 hours for security reason)</p>";
        body += EmailFooter();
        body = "<font size=\"+1\">" + body + "</font>";
        return body;
    }
    private string EmailBodyCreateReq(IdentityUser user)
    {
        string url = $"{State.AppUrl.ToLower()}Identity/AutoLogin";
        url += $"?UserName={user.UserName}&Email={user.Email}";
        url += $"&UserId={clsAccountHash.GetHashedUserId(user.Email!, DateTime.Now)}";

        string body = $"<h3>Hello {user.UserName}-san!</h3>";
        body += $"<p>Thank you for creating your account for {State.AppTitle}.";
        body += $@" Use this <a href=""{url}"">Link</a> to Login to the site.";
        body += "<br/>(The above link works only upto 24hours for security reason)</p>";
        body += EmailFooter();
        body = "<font size=\"+1\">" + body + "</font>";
        return body;
    }
    private string EmailFooter()
    {
        var ft = "<br/>";
        ft += "==================================================<br/>";
        ft += $"{State.AppName}<br/>";
        ft += $"URL: {State.AppUrl}<br/>";
        ft += "==================================================<br/>";
        return ft;
    }

}