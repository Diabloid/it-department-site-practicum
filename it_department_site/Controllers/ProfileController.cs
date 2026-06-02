using Microsoft.AspNetCore.Authorization;

[Route("profile")]
[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IPageService _pageService;
    private readonly ICurrentUserService _currentUser;

    public ProfileController(IProfileService profileService, IPageService pageService, ICurrentUserService currentUser)
    {
        _profileService = profileService;
        _pageService = pageService;
        _currentUser = currentUser;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        int userId = _currentUser.UserId.Value;

        var user = await _profileService.GetUserInfoAsync(userId);
        if (user == null) return RedirectToAction("login", "Auth");

        ViewBag.Username = user.Username;
        ViewBag.Email = user.Email;
        ViewBag.Registered = user.CreatedAt.ToString("dd.MM.yyyy HH:mm");
        return View();
    }
}