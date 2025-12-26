namespace MyCommLib.Client.Classes;

using Blazored.LocalStorage;
using MyCommLib.Shared;
using System.Threading.Tasks;

public class clsRememberMeW {
    const string cLSUser = "User";
    const string cLSPassword = "rmId";
    private ILocalStorageService BLS;

    public clsRememberMeW(ILocalStorageService bls)
    {
        BLS = bls;
    }
    public async Task Set(string usr, string pwd) {
        await BLS.SetItemAsync($"{MyAppInfo.AppTitle}-{cLSUser}", usr);
        await BLS.SetItemAsync($"{MyAppInfo.AppTitle}-{cLSPassword}", pwd);
    }
    public async Task<(string usr, string pwd)> Get() {
        var u = await BLS.GetItemAsync<string>($"{MyAppInfo.AppTitle}-{cLSUser}");
        var p = await BLS.GetItemAsync<string>($"{MyAppInfo.AppTitle}-{cLSPassword}");
        return (u!, p!);
    }
    public async Task Reset() {
        await BLS.SetItemAsync($"{MyAppInfo.AppTitle}-{cLSUser}", "");
        await BLS.SetItemAsync($"{MyAppInfo.AppTitle}-{cLSPassword}", "");
    }
}
