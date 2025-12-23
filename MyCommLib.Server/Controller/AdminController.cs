namespace MyCommLib.Server.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MyCommLib.Classes;
using MyCommLib.Server.Classes;
using System.Collections.Generic;

[Route("api/[controller]/[action]")]
[ApiController]
public class AdminController : ControllerBase
{
    private IConfiguration _config { get; set; }
    private IHostEnvironment _env;
    public AdminController(IConfiguration config, IHostEnvironment env)
    {
        _config = config;
        _env = env;
    }
    [HttpGet("{name}")]
    public async Task<clsKVP> GetConfig(string name)
    {
        await Task.CompletedTask;
        var val = _config.GetValue<string>(name);
        var kvp = new clsKVP(name, val!);
        return kvp;
    }
    [HttpGet]
    public string EnvName() => _env.EnvironmentName;
    [HttpGet]
    public bool IsDevelopment() => _env.IsDevelopment();
    const string cEncPassword = "Koreha himitsu no aikotoba";
    [HttpPost]
    public string GetEncrypted(List<string> list)
    {
        var original = list[0];
        var password = (list.Count > 1) ? list[1] : cEncPassword;
        var encrypted = clsEncryption.Encrypt(original, password);
        return encrypted;
    }
    [HttpPost]
    public string GetDecrypted(List<string> list)
    {
        var encrypted = list[0];
        var password = (list.Count > 1) ? list[1] : cEncPassword;
        var decrypted = clsEncryption.Decrypt(encrypted, password);
        return decrypted;
    }
}