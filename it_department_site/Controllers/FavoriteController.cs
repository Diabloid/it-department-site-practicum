using Microsoft.AspNetCore.Mvc;
using it_department_site.Filters;
using it_department_site.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[Route("favorites")]
[JwtAuthorize("User")]
public class FavoriteController : Controller
{
    private readonly IFavoriteService _favoriteService;
    private readonly IPageService _pageService;

    public FavoriteController(IFavoriteService favoriteService, IPageService pageService)
    {
        _favoriteService = favoriteService;
        _pageService = pageService;
    }
    [HttpPost("remove")]
    public IActionResult Remove(int pageId)
    {
        var token = Request.Cookies["jwt"];
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        int userId = int.Parse(jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");

        _favoriteService.RemoveFromFavorites(userId, pageId);

        return RedirectToAction("Index", "Profile");
    }
    [HttpPost("toggle")]
    public JsonResult Toggle(int pageId)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(Request.Cookies["jwt"]);
        var userId = int.Parse(jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        bool added = _favoriteService.ToggleFavorite(userId, pageId); // додав → true, видалив → false

        return Json(new { success = true, isFavorite = added });
    }
}
