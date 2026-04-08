
using System.Security.Claims;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;

    public AuthController(IAuthService authService, IConfiguration config)
    {
        _authService = authService;
        _config = config;
    }

    // REFACTOR_HINT: Configure custom login path in Program.cs
    [HttpGet("Account/Login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        // ✅ await
        var user = await _authService.LoginAsync(username, password);

        if (user != null)
        {
            string role = user.Username.ToLower() == "admin" ? "Admin" : "User";
        

            return role == "Admin"
                ? RedirectToAction("Index", "Admin")
                : RedirectToAction("Index", "Pages");
        }

        ViewBag.Error = "Невірний логін або пароль";
        return View();
    }

    public IActionResult Logout()
    {
        return RedirectToAction("Index", "Pages");
    }

    
}