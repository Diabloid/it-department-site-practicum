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
}
