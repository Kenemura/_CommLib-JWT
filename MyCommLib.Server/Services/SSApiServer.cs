using Microsoft.EntityFrameworkCore;
using MyCommLib.Classes;
using MyCommLib.Server.Data;
using MyCommLib.Shared.Interfaces;
using MyCommLib.Shared.Models;

namespace MyCommLib.Server.Services;
public class SSApiServer : ISSApi
{
    private SSData dc;
    public SSApiServer(SSData dc)
    {
        this.dc = dc;
    }
    public async Task<IEnumerable<SessionStore>> GetList()
    {
        await Task.CompletedTask;
        var data = dc.SessionStores.OrderBy(x => x.Id).ToList();
        return data;
    }
    public async Task<SessionStore> Get(string id)
    {
        var item = await dc.SessionStores.FirstOrDefaultAsync(x => x.Id == id);
        return item ?? new SessionStore();
    }
    public async Task<SessionStore> Get(string sId, string key)
    {
        var item = await dc.SessionStores.FirstOrDefaultAsync(x => x.SessionId == sId && x.Key == key);
        return item ?? new SessionStore();
    }
    public async Task<string> Create(SessionStore edited)
    {
        var item = await dc.SessionStores.FirstOrDefaultAsync(x => x.Id == edited.Id);
        if (item is not null) throw new Exception("Id already exists");
        dc.SessionStores.Add(edited);
        DeleteExpiredData();
        dc.SaveChanges();
        return edited.Id;
    }
    public async Task Update(SessionStore edited)
    {
        var item = await dc.SessionStores.FirstOrDefaultAsync(x => x.Id == edited.Id);
        if (item is null) throw new Exception("SS not found");
        edited.CopyTo(item);
        dc.SaveChanges();
    }
    public async Task Delete(SessionStore edited)
    {
        var item = await dc.SessionStores.FirstOrDefaultAsync(x => x.Id == edited.Id);
        if (item is null) throw new Exception("SS not found");
        dc.SessionStores.Remove(item);
        dc.SaveChanges();
    }

    public async Task SetValue<T>(string sId, string key, T value) where T : class
    {
        var json = new clsJson<T>().Serialize(value);
        var ss = await dc.SessionStores.FirstOrDefaultAsync(x => x.SessionId == sId && x.Key == key);
        if (ss is null)
        {
            ss = new SessionStore() { SessionId = sId!, Key = key, Value = json };
            dc.SessionStores.Add(ss);
        }
        else
        {
            ss.Value = json;
            dc.SessionStores.Update(ss);
        }
        dc.SaveChanges();
    }
    public async Task<T> GetValue<T>(string sId, string key) where T : class
    {
        var ss = await Get(sId, key);
        var value = new clsJson<T>().Deserialize(ss.Value!);
        return value;
    }

    private void DeleteExpiredData()
    {
        var items = dc.SessionStores.Where(x => x.TimeCreated < DateTime.UtcNow.AddDays(-1));
        if (items.Any()) dc.RemoveRange(items);
    }
}
