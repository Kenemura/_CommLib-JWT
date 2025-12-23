using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;

namespace MyCommLib.Client.Services;

public class MyAuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _storage;
    public MyAuthorizationMessageHandler(ILocalStorageService storage)
    {
        _storage = storage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage req, CancellationToken cancellationToken)
    {
        if (await _storage.ContainKeyAsync("access_token"))
        {
            var token = await _storage.GetItemAsync<string>("access_token");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(req, cancellationToken);
    }
}
