namespace MyCommLib.Client.Services;

using System.Net.Http.Json;

public class ExcelApi
{
    private readonly HttpClient Http;
    public ExcelApi(HttpClient http)
    {
        Http = http;
    }
    public async Task<byte[]> DownloadTemplate(string name)
    {
        var result = await Http.GetAsync($"api/excel/downloadtemplate/{name}");
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }
        var bytes = await result.Content.ReadAsByteArrayAsync();
        return bytes;
    }
}