namespace MyCommLib.Server.Services;

using Microsoft.AspNetCore.Http;
using MyCommLib.Shared.Interfaces;

public class StateSetUserMiddleware
{
    private readonly RequestDelegate next;

    public StateSetUserMiddleware(RequestDelegate next)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context, IState state)
    {
        state.SetUser(context.User);
        await next(context);
    }
}