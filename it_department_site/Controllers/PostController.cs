namespace it_department_site.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;

        public PostController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _postRepository.GetAllAsync();
            return View(posts);
        }

        [HttpGet("/posts/{slug}-{id:int}")]
        public async Task<IActionResult> Details(string slug, int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            return post == null || !string.Equals(post.Slug, slug, StringComparison.OrdinalIgnoreCase)
                ? View("~/Views/Shared/NotFound.cshtml")
                : View(post);
        }
    }
}