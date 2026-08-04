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

    [HttpGet("/Home/StatusCode")]
    public IActionResult StatusCode(int code)
    {
        Response.StatusCode = code;
        if (code == 404)
            return View("~/Views/Shared/NotFound.cshtml");

        ViewData["Title"] = "Помилка";
        return View("~/Views/Shared/Error.cshtml", new ErrorViewModel());
    }

    [HttpGet("/not-found")]
    public IActionResult NotFoundPage()
    {
        Response.StatusCode = 404;
        return View("~/Views/Shared/NotFound.cshtml");
    }
}
