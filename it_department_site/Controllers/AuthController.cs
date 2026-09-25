using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace it_department_site.Controllers;

public class AuthController : Controller
{
    [HttpGet("/login")]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Admin");
        }

        return Challenge(
            new AuthenticationProperties { RedirectUri = Url.Action("Index", "Admin") },
            OpenIdConnectDefaults.AuthenticationScheme
        );
    }

    [HttpGet("/logout")]
    public IActionResult Logout()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme
        );
    }
}