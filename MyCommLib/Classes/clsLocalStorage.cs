using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace MyCommLib.Classes;
public class clsLocalStorage<T> {
    private IJSRuntime JS;
    public clsLocalStorage(IJSRuntime js) {
        JS = js;
    }
    public async Task Set(string name, object value) {
        await JS.InvokeVoidAsync("blazorLocalStorage.set", name, value);
    }
    public async Task<(bool, T?)> Get(string name) {
        try {
            var c = await JS.InvokeAsync<T>("blazorLocalStorage.get", name);
            if (c is not null) return (true, c);
        } catch (Exception ex) {
            Console.WriteLine(ex);
        }
        return (false, default(T));
    }
    public async Task<T> Get(string name, T defaultValue) {
        try {
            var c = await JS.InvokeAsync<T>("blazorLocalStorage.get", name);
            return c;
        } catch { }
        return defaultValue;
    }
    public async Task Delete(string name) {
        var c = await JS.InvokeAsync<T>("blazorLocalStorage.delete", name);
    }
}