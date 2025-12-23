using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace MyCommLib.Classes
{
    public static class clsJsExtentions
    {
        public static ValueTask<object> SaveAs(this IJSRuntime js, string filename, byte[] data)
            => js.InvokeAsync<object>(
               "saveAsFile",
               filename,
               Convert.ToBase64String(data));

        public static ValueTask<object> SaveAs(this IJSRuntime js, string filename, string data)
            => js.InvokeAsync<object>(
               "saveAsFile",
               filename,
               Convert.ToBase64String(GetBytes(data)));

        private static byte[] GetBytes(string data)
            => System.Text.Encoding.ASCII.GetBytes(data);
    }
}
