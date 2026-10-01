using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace it_department_site.Services;

public class EntraIdAuthenticationEvents(IUserService userService) : OpenIdConnectEvents
{
    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value ??
                    context.Principal?.FindFirst("preferred_username")?.Value;

        if (string.IsNullOrEmpty(email))
        {
            context.Fail("Email не знайдено в токені.");
            return;
        }

        var dbUser = await userService.FindUserByEmailAsync(email);

        if (dbUser == null)
        {
            context.Fail("Користувача не знайдено в базі даних.");
            return;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, dbUser.Id.ToString()),
            new(ClaimTypes.Name, dbUser.Username)
        };

        claims.AddRange(dbUser.Roles.Select(r => new Claim(ClaimTypes.Role, r.ToString())));

        var appIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        context.Principal?.AddIdentity(appIdentity);
    }

    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        context.Response.Redirect("/Error");
        context.HandleResponse();

        return Task.CompletedTask;
    }
}