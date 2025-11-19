using it_department_site.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;

    public AuthController(IAuthService authService, IConfiguration config)
    {
        _authService = authService;
        _config = config;
    }

    [HttpGet("/login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("/login")]
    public IActionResult Login(string username, string password)
    {
        var user = _authService.Login(username, password);

        if (user != null)
        {
            int userId = user.Id;
            string role = user.Username.ToLower() == "admin" ? "Admin" : "User";

            var token = GenerateJwtToken(user.Id, user.Username, role);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddHours(1)
            });

            return role == "Admin"
                ? RedirectToAction("Index", "Admin")
                : RedirectToAction("Index", "Pages");
        }

        ViewBag.Error = "Невірний логін або пароль";
        return View();
    }

    [HttpGet("/register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("/register")]
    public IActionResult Register(string username, string email, string password)
    {
        if (_authService.IsUsernameTaken(username))
        {
            ViewBag.Error = "Такий користувач вже існує!";
            return View();
        }

        _authService.Register(username, email, password);
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return RedirectToAction("Index", "Pages");
    }

    private string GenerateJwtToken(int userId, string username, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
