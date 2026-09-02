using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("/login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _authService.LoginAsync(username, password);

        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r.ToString())));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity)
            );
            return user.Roles.Contains(UserRole.Admin)
                ? RedirectToAction("Index", "Admin")
                : RedirectToAction("Index", "Pages");
        }

        ViewBag.Error = "Невірний логін або пароль";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Pages");
    }

}