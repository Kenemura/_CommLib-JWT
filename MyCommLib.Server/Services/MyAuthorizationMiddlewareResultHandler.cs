namespace MyCommLib.Server.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

// ** Comment by Ken **
// This middleware is used to handle Google authentication to use Google api ???

public class MyAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();
    public Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        {
            if (authorizeResult.Succeeded)
            {
                return next(context);
            }

            return Handle();

            async Task Handle()
            {
                if (authorizeResult.Challenged)
                {
                    if (policy.AuthenticationSchemes.Count > 0)
                    {
                        foreach (var scheme in policy.AuthenticationSchemes)
                        {
                            await context.ChallengeAsync(scheme);
                        }
                    }
                    else
                    {
                        //await context.ChallengeAsync();
                        await next(context);
                    }
                }
                else if (authorizeResult.Forbidden)
                {
                    if (policy.AuthenticationSchemes.Count > 0)
                    {
                        foreach (var scheme in policy.AuthenticationSchemes)
                        {
                            await context.ForbidAsync(scheme);
                        }
                    }
                    else
                    {
                        //await context.ForbidAsync();
                        await next(context);
                    }
                }
            }
        }
        //if (policy.AuthenticationSchemes.Count == 0)
        //{
        //    return next(context);
        //}
        //else
        //{
        //    return defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        //}
    }
}