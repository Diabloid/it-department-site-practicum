namespace it_department_site.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetAllAsync();
            return View(posts);
        }

        [HttpGet("/posts/{slug}-{id:int}")]
        public async Task<IActionResult> Details(string slug, int id)
        {
            var posts = await _postService.GetLatestAsync(5);
            ViewBag.Posts = posts.Where(post => post.Id != id);

            var post = await _postService.GetByIdAsync(id);
            if (post == null || !string.Equals(post.Slug, slug, StringComparison.OrdinalIgnoreCase))
            {
		Response.StatusCode = 404;
                return View("~/Views/Shared/NotFound.cshtml");
            }

            // ЛОГІКА ПЕРЕГЛЯДІВ (COOKIES)
            string viewCookieName = $"viewed_post_{post.Id}";

            if (!Request.Cookies.ContainsKey(viewCookieName))
            {
                await _postService.IncrementViewsAsync(post.Id);
                post.ViewsCount++;

                CookieOptions options = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(24),
                    HttpOnly = true,                              
                    IsEssential = true
                };

                Response.Cookies.Append(viewCookieName, "true", options);
            }

            ViewBag.Views = post.ViewsCount;
            return View(post);
        }

    }
}
