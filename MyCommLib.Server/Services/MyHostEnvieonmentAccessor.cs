using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MyCommLib.Server.Services;
public class MyHostEnvironmentAccessor
{
    private IWebHostEnvironment env = default!;
    public MyHostEnvironmentAccessor(IWebHostEnvironment env)
    {
        this.env = env;
    }
    public string EnvironmentName => env.EnvironmentName;
    public bool IsDevelopment => env.IsDevelopment();
}
