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

        [HttpGet("/posts/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            var post = await _postService.GetBySlugAsync(slug);
            if (post == null)
            {
                return NotFound();
            }

            var posts = await _postService.GetLatestAsync(5, post.Id);
            ViewBag.Posts = posts;

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