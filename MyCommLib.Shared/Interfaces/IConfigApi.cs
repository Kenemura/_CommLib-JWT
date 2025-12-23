using MyCommLib.Shared.Models;

namespace MyCommLib.Shared.Interfaces;

public interface IConfigApi
{
    Task<IEnumerable<ConfigKVP>> GetList();
    Task<ConfigKVP> Get(Guid id);
    Task<Guid> Create(ConfigKVP edited);
    Task Update(ConfigKVP edited);
    Task Delete(ConfigKVP edited);
}