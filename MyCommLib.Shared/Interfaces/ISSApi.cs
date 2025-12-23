using MyCommLib.Shared.Models;

namespace MyCommLib.Shared.Interfaces;
public interface ISSApi
{
    Task<IEnumerable<SessionStore>> GetList();
    Task<SessionStore> Get(string id);
    Task<SessionStore> Get(string sId, string key);
    Task<string> Create(SessionStore edited);
    Task Update(SessionStore edited);
    Task Delete(SessionStore edited);
    Task<T> GetValue<T>(string sId, string key) where T : class;
    Task SetValue<T>(string sId, string key, T value) where T : class;
}