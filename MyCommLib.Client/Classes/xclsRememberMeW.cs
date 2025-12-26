namespace MyCommLib.Client.Classes;

using Microsoft.JSInterop;
using MyCommLib.Classes;
using MyCommLib.Shared;
using System.Threading.Tasks;

public class xclsRememberMeW {
    private IJSRuntime JS;
    private clsLocalStorage<string> LS;
    const string cLSUser = "User";
    const string cLSPassword = "rmId";
    public xclsRememberMeW(IJSRuntime js) {
        JS = js;
        LS = new clsLocalStorage<string>(JS);
    }
    public async Task Set(string usr, string pwd) {
        await LS.Set($"{MyAppInfo.AppTitle}-{cLSUser}", usr);
        await LS.Set($"{MyAppInfo.AppTitle}-{cLSPassword}", pwd);
    }
    public async Task<(string usr, string pwd)> Get() {
        var u = await LS.Get($"{MyAppInfo.AppTitle}-{cLSUser}", "");
        var p = await LS.Get($"{MyAppInfo.AppTitle}-{cLSPassword}", "");
        return (u, p);
    }
    public async Task Reset() {
        await LS.Set($"{MyAppInfo.AppTitle}-{cLSUser}", "");
        await LS.Set($"{MyAppInfo.AppTitle}-{cLSPassword}", "");
    }
}
