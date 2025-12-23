namespace MyCommLib.Client.Services;

using MyCommLib.Shared.Models;
using System.Net.Http.Json;
public class ConfigApi
{
    private readonly HttpClient Http;
    public ConfigApi(HttpClient http)
    {
        Http = http;
    }
    public async Task<IEnumerable<ConfigKVP>> GetList()
    {
        //var result = await Http.GetAsync("api/Config/GetListBugFix");
        var result = await Http.GetAsync("api/Config/GetList");
        var data = (await result.Content.ReadFromJsonAsync<IEnumerable<ConfigKVP>>())?.ToList() ?? new List<ConfigKVP>();
        return data;
    }
    public async Task<ConfigKVP> Get(Guid id)
    {
        var result = await Http.GetFromJsonAsync<ConfigKVP>($"api/Config/get/{id}");
        return result!;
    }
    public async Task<Guid> Create(ConfigKVP edited)
    {
        var result = await Http.PostAsJsonAsync($"api/Config/Create", edited);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
        return edited.Id;
    }
    public async Task Update(ConfigKVP edited)
    {
        var result = await Http.PostAsJsonAsync($"api/Config/Update", edited);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
    public async Task Delete(ConfigKVP edited)
    {
        var result = await Http.PostAsJsonAsync($"api/Config/Delete", edited);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
    }
}
