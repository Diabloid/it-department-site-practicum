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
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "partners");
            var orderFilePath = Path.Combine(folderPath, "order.json");
            List<string> finalPartnerList = new List<string>();

            if (Directory.Exists(folderPath))
            {
                var dirInfo = new DirectoryInfo(folderPath);

                var allFiles = dirInfo.GetFiles()
                                      .Where(f => !f.Name.EndsWith("order.json"))
                                      .OrderBy(f => f.CreationTime)
                                      .Select(f => "/images/partners/" + f.Name)
                                      .ToList();

                if (System.IO.File.Exists(orderFilePath))
                {
                    try
                    {
                        var json = System.IO.File.ReadAllText(orderFilePath);
                        var savedOrder = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json);
                        finalPartnerList = savedOrder.Where(f => allFiles.Contains(f)).ToList();
                    }
                    catch { }
                }

                foreach (var file in allFiles)
                {
                    if (!finalPartnerList.Contains(file)) finalPartnerList.Add(file);
                }
            }

            ViewBag.Partners = finalPartnerList;
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

        [HttpGet]
        public async Task<JsonResult> SearchSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new List<object>());

            var pages = await _pageService.SearchAsync(term);
            var results = pages
                .Select(p => new { title = p.Title, slug = p.Slug })
                .Distinct()
                .Take(10)
                .ToList();

            return Json(results);
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