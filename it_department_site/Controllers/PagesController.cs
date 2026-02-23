namespace it_department_site.Controllers
{
    public class PagesController : Controller
    {
        private readonly IPageService _pageService;
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;
        private readonly ICurrentUserService _currentUser;

        public PagesController(IPageService pageService, ICommentService commentService, ILikeService likeService, ICurrentUserService currentUser)
        {
            _pageService = pageService;
            _commentService = commentService;
            _likeService = likeService;
            _currentUser = currentUser;
        }

        public async Task<IActionResult> Index()
        {
            var pages = await _pageService.GetAllPagesAsync();
            var dtoList = pages.Select(PageMapper.ToDto).ToList();
            return View(dtoList);
        }

        public async Task<IActionResult> Details(string slug)
        {
            bool isAdmin = _currentUser.IsAdmin;

            var page = await _pageService.GetPageBySlugAsync(slug, isAdmin);
            if (page == null)
                return View("~/Views/Shared/NotFound.cshtml");

            if (_currentUser.IsAuthenticated)
            {
                int userId = _currentUser.UserId.Value;
                if (!await _pageService.HasUserViewedPageAsync(userId, page.Id))
                {
                    await _pageService.MarkPageAsViewedAsync(userId, page.Id);
                }
            }

            ViewBag.Comments = await _commentService.GetCommentsAsync(page.Id);
            ViewBag.Likes = await _likeService.GetLikeCountAsync(page.Id);
            ViewBag.Views = await _pageService.GetViewsAsync(page.Id);

            var dto = PageMapper.ToDto(page);
            return View(dto);
        }


        // UploadImage залишається таким самим (він вже був async)
        [HttpPost]
        [Route("ckeditor/upload")]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }

                var fileUrl = Url.Content($"~/uploads/{fileName}");

                return Json(new { uploaded = true, url = fileUrl });
            }
            return Json(new { uploaded = false, error = new { message = "Error" } });
        }
    }
}