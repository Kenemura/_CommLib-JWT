namespace MyCommLib.Client.Classes;

using Microsoft.JSInterop;
using MyCommLib.Classes;
using MyCommLib.Shared.Services;
using System.Threading.Tasks;

public class clsRememberMeW {
    private HttpClient Http;
    private IJSRuntime JS;
    private clsLocalStorage<string> LS;
    //const string cLSName = "rmId";
    const string cLSUser = "User";
    const string cLSPassword = "rmId";
    public clsRememberMeW(HttpClient http, IJSRuntime js) {
        Http = http;
        JS = js;
        LS = new clsLocalStorage<string>(JS);
    }
    public async Task Set(string usr, string pwd) {
        await LS.Set($"{State.AppTitle}-{cLSUser}", usr);
        await LS.Set($"{State.AppTitle}-{cLSPassword}", pwd);
    }
    public async Task<(string usr, string pwd)> Get() {
        var u = await LS.Get($"{State.AppTitle}-{cLSUser}", "");
        var p = await LS.Get($"{State.AppTitle}-{cLSPassword}", "");
        return (u, p);
    }
    public async Task Reset() {
        await LS.Set($"{State.AppTitle}-{cLSUser}", "");
        await LS.Set($"{State.AppTitle}-{cLSPassword}", "");
    }
}
