using it_department_site.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IPostRepository _postRepository;

    public HomeController(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.News = await _postRepository.GetLatestAsync(4);
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
