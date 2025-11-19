using Microsoft.AspNetCore.Mvc;
using it_department_site.Filters;
using it_department_site.Services;
using it_department_site.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


[Route("profile")]
[JwtAuthorize("User")]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly ILikeService _likeService;
    private readonly IPageService _pageService;

    public ProfileController(IProfileService profileService, ILikeService likeService, IPageService pageService)
    {
        _profileService = profileService;
        _likeService = likeService;
        _pageService = pageService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var token = Request.Cookies["jwt"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return RedirectToAction("Login", "Auth");

        int userId = int.Parse(userIdClaim.Value);

        // 🔽 Тепер тут повертається об'єкт User?
        var user = _profileService.GetUserInfo(userId);
        if (user == null) // 🔽 Перевірка стала простішою
            return RedirectToAction("Login", "Auth");

        ViewBag.Username = user.Username;
        ViewBag.Email = user.Email;
        ViewBag.Registered = user.CreatedAt.ToString("dd.MM.yyyy HH:mm");

        // 🔽 Ось тут нова логіка
        var favorites = _profileService.GetFavorites(userId); // List<Favorite>
        var favoriteViewModels = new List<FavoritePageViewModel>();

        foreach (var fav in favorites)
        {
            var page = _pageService.GetPageById(fav.PageId);
            if (page != null)
            {
                favoriteViewModels.Add(new FavoritePageViewModel
                {
                    PageId = page.Id,
                    PageTitle = page.Title,
                    PageSlug = page.Slug,
                    CreatedAt = page.CreatedAt,
                    Views = page.Views,
                    Likes = _likeService.GetLikeCount(page.Id)
                });
            }
        }

        ViewBag.Favorites = favoriteViewModels;

        return View();
    }
}
