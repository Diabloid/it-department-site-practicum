namespace it_department_site.Controllers
{
    public class HomeController(IPostService postService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.News = await postService.GetLatestAsync(4);
            return View();
        }
    }
}
