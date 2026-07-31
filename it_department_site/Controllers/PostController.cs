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
            return post == null || !string.Equals(post.Slug, slug, StringComparison.OrdinalIgnoreCase)
                ? View("~/Views/Shared/NotFound.cshtml")
                : View(post);
        }

        
    }
}