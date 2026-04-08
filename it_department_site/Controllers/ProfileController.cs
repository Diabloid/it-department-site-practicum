using Microsoft.AspNetCore.Authorization;

[Route("profile")]
[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly ILikeService _likeService;
    private readonly IPageService _pageService;
    private readonly ICurrentUserService _currentUser;

    public ProfileController(IProfileService profileService, ILikeService likeService, IPageService pageService, ICurrentUserService currentUser)
    {
        _profileService = profileService;
        _likeService = likeService;
        _pageService = pageService;
        _currentUser = currentUser;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        int userId = _currentUser.UserId.Value;

        var user = await _profileService.GetUserInfoAsync(userId);
        if (user == null) return RedirectToAction("Login", "Auth");

        ViewBag.Username = user.Username;
        ViewBag.Email = user.Email;
        ViewBag.Registered = user.CreatedAt.ToString("dd.MM.yyyy HH:mm");

        var favorites = await _profileService.GetFavoritesAsync(userId);
        var favoriteViewModels = new List<FavoritePageViewModel>();

        foreach (var fav in favorites)
        {
            var page = await _pageService.GetPageByIdAsync(fav.PageId);
            if (page != null)
            {
                favoriteViewModels.Add(new FavoritePageViewModel
                {
                    PageId = page.Id,
                    PageTitle = page.Title,
                    PageSlug = page.Slug,
                    CreatedAt = page.CreatedAt,
                    Views = page.Views,
                    Likes = await _likeService.GetLikeCountAsync(page.Id)
                });
            }
        }

        ViewBag.Favorites = favoriteViewModels;
        return View();
    }
}