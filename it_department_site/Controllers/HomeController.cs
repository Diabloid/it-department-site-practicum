using it_department_site.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IPostService _postService;

    public HomeController(IPostService postService)
    {
        _postService = postService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.News = await _postService.GetLatestAsync(4);
        return View();
    }
}
