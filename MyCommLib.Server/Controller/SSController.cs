using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using MyCommLib.Server.Data;
using MyCommLib.Shared.Interfaces;
using MyCommLib.Shared.Models;

namespace MyCommLib.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class SSController : ControllerBase
{
    private SSData dc;
    private ISSApi api;
    public SSController(SSData dc, ISSApi api)
    {
        this.dc = dc; 
        this.api = api;
    }
    [HttpGet]
    public async Task<IEnumerable<SessionStore>> Getlist()
    {
        return await api.GetList();
    }
    [HttpGet("{id}")]
    public async Task<SessionStore> Get(string id)
    {
        return await api.Get(id);
    }
    [HttpGet("{sId}/{key}")]
    public async Task<SessionStore> Get(string sId, string key)
    {
        return await api.Get(sId, key);
    }
    [HttpPost]
    public async Task<IActionResult> CreateUpdate(SessionStore item)
    {
        try
        {
            var ss = await api.Get(item.SessionId!, item.Key!);
            if ( ss.SessionId != item.SessionId || ss.Key != item.Key) // not found
            {
                await api.Create(item);
            }
            else
            {
                ss.Value = item.Value;
                await api.Update(ss);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
