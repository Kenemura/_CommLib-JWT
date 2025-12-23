using Microsoft.EntityFrameworkCore;
using MyCommLib.Classes;
using MyCommLib.Server.Data;
using MyCommLib.Shared.Interfaces;
using MyCommLib.Shared.Models;

namespace MyCommLib.Server.Services;
public class ConfigApi : IConfigApi
{
    private ConfigData dc;
    public ConfigApi(ConfigData dc)
    {
        this.dc = dc;
    }
    public async Task<IEnumerable<ConfigKVP>> GetList()
    {
        await Task.CompletedTask;
        var data = dc.ConfigKVPs.OrderBy(x => x.Key).ToList();
        return data;
    }
    public async Task<ConfigKVP> Get(Guid id)
    {
        var item = await dc.ConfigKVPs.FirstOrDefaultAsync(x => x.Id == id);
        return item ?? new ConfigKVP();
    }
    public async Task<Guid> Create(ConfigKVP edited)
    {
        var item = await dc.ConfigKVPs.FirstOrDefaultAsync(x => x.Key == edited.Key);
        if (item is not null) throw new Exception("Key already exists");
        dc.ConfigKVPs.Add(edited);
        dc.SaveChanges();
        return edited.Id;
    }
    public async Task Update(ConfigKVP edited)
    {
        var item = await dc.ConfigKVPs.FirstOrDefaultAsync(x => x.Id == edited.Id);
        if (item is null) throw new Exception("Config not found");
        edited.CopyTo(item, true);
        dc.Entry(item).State = EntityState.Modified;
        dc.SaveChanges();
    }
    public async Task Delete(ConfigKVP edited)
    {
        var item = await dc.ConfigKVPs.FirstOrDefaultAsync(x => x.Id == edited.Id);
        if (item is null) throw new Exception("Config not found");
        dc.ConfigKVPs.Remove(item);
        dc.SaveChanges();
    }
}
