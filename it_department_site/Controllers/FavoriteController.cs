using Microsoft.AspNetCore.Mvc;
using it_department_site.Filters;
using it_department_site.Services;

[Route("favorites")]
[JwtAuthorize("User")]
public class FavoriteController : Controller
{
    private readonly IFavoriteService _favoriteService;
    private readonly IPageService _pageService;
    private readonly ICurrentUserService _currentUser; 

    public FavoriteController(IFavoriteService favoriteService, IPageService pageService, ICurrentUserService currentUser)
    {
        _favoriteService = favoriteService;
        _pageService = pageService;
        _currentUser = currentUser;
    }

    [HttpPost("remove")]
    public IActionResult Remove(int pageId)
    {
        // ✅ Використовуємо .Value, бо атрибут [JwtAuthorize] гарантує, що юзер залогінений
        int userId = _currentUser.UserId.Value;

        _favoriteService.RemoveFromFavorites(userId, pageId);

        return RedirectToAction("Index", "Profile");
    }

    [HttpPost("toggle")]
    public JsonResult Toggle(int pageId)
    {
        int userId = _currentUser.UserId.Value;

        bool added = _favoriteService.ToggleFavorite(userId, pageId);

        return Json(new { success = true, isFavorite = added });
    }
}