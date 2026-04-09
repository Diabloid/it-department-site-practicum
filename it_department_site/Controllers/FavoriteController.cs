using Microsoft.AspNetCore.Authorization;

[Route("favorites")]
[Authorize]
public class FavoriteController : Controller
{
    private readonly IFavoriteService _favoriteService;
    private readonly ICurrentUserService _currentUser;

    public FavoriteController(IFavoriteService favoriteService, ICurrentUserService currentUser)
    {
        _favoriteService = favoriteService;
        _currentUser = currentUser;
    }

    [HttpPost("remove")]
    public async Task<IActionResult> Remove(int pageId)
    {
        int userId = _currentUser.UserId.Value;
        await _favoriteService.RemoveFromFavoritesAsync(userId, pageId);
        return RedirectToAction("Index", "Profile");
    }

    [HttpPost("toggle")]
    public async Task<JsonResult> Toggle(int pageId)
    {
        int userId = _currentUser.UserId.Value;
        bool added = await _favoriteService.ToggleFavoriteAsync(userId, pageId);
        return Json(new { success = true, isFavorite = added });
    }
}