using Microsoft.AspNetCore.Mvc;
using MyCommLib.Server.Data;
using MyCommLib.Shared.Interfaces;
using MyCommLib.Shared.Models;

namespace MyCommLib.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ConfigController : ControllerBase
{
    private ConfigData dc;
    private IConfigApi api;
    public ConfigController(ConfigData dc, IConfigApi api)
    {
        this.dc = dc; 
        this.api = api;
    }
    [HttpGet]
    public async Task<IEnumerable<ConfigKVP>> Getlist()
    {
        return await api.GetList();
    }
    [HttpGet("{id:guid}")]
    public async Task<ConfigKVP> Get(Guid id)
    {
        return await api.Get(id);
    }
    [HttpPost]
    public async Task<IActionResult> Create(ConfigKVP item)
    {
        try
        {
            var result = await api.Create(item);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost]
    public async Task<IActionResult> Update(ConfigKVP item)
    {
        try
        {
            await api.Update(item);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost]
    public async Task<IActionResult> Delete(ConfigKVP item)
    {
        try
        {
            await api.Delete(item);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
