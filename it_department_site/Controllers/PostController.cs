namespace it_department_site.Controllers
{
    public class PostController(IPostService postService) : Controller
    {
        [HttpGet("/posts")]
        public async Task<IActionResult> Index([FromQuery] string? category)
        {
            var posts = category == null 
                ? await postService.GetAllAsync()
                : await postService.GetByCategoryName(category);
            return View(posts);
        }

        [HttpGet("/posts/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            var post = await postService.GetBySlugAsync(slug);
            if (post == null)
            {
                return NotFound();
            }

            var posts = await postService.GetLatestAsync(5, post.Id);
            ViewBag.Posts = posts;

            // ЛОГІКА ПЕРЕГЛЯДІВ (COOKIES)
            string viewCookieName = $"viewed_post_{post.Id}";
            if (!Request.Cookies.ContainsKey(viewCookieName))
            {
                await postService.IncrementViewsAsync(post.Id);
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
