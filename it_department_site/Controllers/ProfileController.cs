using Microsoft.AspNetCore.Mvc;
using it_department_site.Filters;
using it_department_site.Services;
using it_department_site.Models;

[Route("profile")]
[JwtAuthorize("User")]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly ILikeService _likeService;
    private readonly IPageService _pageService;
    private readonly ICurrentUserService _currentUser; // ✅

    public ProfileController(IProfileService profileService, ILikeService likeService, IPageService pageService, ICurrentUserService currentUser)
    {
        _profileService = profileService;
        _likeService = likeService;
        _pageService = pageService;
        _currentUser = currentUser;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        // Атрибут [JwtAuthorize] вже перевірив токен, тому просто беремо ID
        int userId = _currentUser.UserId.Value;

        var user = _profileService.GetUserInfo(userId);
        if (user == null)
            return RedirectToAction("Login", "Auth");

        ViewBag.Username = user.Username;
        ViewBag.Email = user.Email;
        ViewBag.Registered = user.CreatedAt.ToString("dd.MM.yyyy HH:mm");

        var favorites = _profileService.GetFavorites(userId);
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