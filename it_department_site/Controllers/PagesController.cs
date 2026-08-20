namespace it_department_site.Controllers
{
    public class PagesController : Controller
    {
        private readonly IPageService _pageService;
        private readonly ICurrentUserService _currentUser;
        private readonly IPartnerService _partnerService;
        private readonly IPostService _postService;
        private readonly IMainSliderService _mainSliderService;

        public PagesController(IPageService pageService, 
            ICurrentUserService currentUser, IPartnerService partnerService, IPostService postService,
            IMainSliderService mainSliderService)
        {
            _pageService = pageService;
            _currentUser = currentUser;
            _partnerService = partnerService;
            _postService = postService;
            _mainSliderService = mainSliderService;
        }

        public async Task<IActionResult> Index()
        {
            var partners = await _partnerService.GetAllPartnersAsync();
            ViewBag.Partners = partners.ToList();
            ViewBag.MainSliderSlides = await _mainSliderService.GetDisplaySlidesAsync();

            // 2. --- ЛОГІКА НОВИН ---
            ViewBag.News = await _postService.GetLatestAsync(4);

            // 3. --- ЛОГІКА СТОРІНОК ---
            var pages = await _pageService.GetAllPagesAsync();
            var dtoList = pages.Select(PageMapper.ToDto).ToList();
            return View(dtoList);
        }

        public async Task<IActionResult> Details(string slug)
        {
            bool isAdmin = _currentUser.IsAuthenticated && _currentUser.IsAdmin;

            var page = await _pageService.GetPageBySlugAsync(slug, isAdmin);
            if (page == null)
            {
                return NotFound();
            }

            // НОВА ЛОГІКА ПЕРЕГЛЯДІВ (COOKIES)
            string viewCookieName = $"viewed_page_{page.Id}";

            if (!Request.Cookies.ContainsKey(viewCookieName))
            {
                await _pageService.IncrementViewsAsync(page.Id);

                page.ViewsCount++;

                CookieOptions options = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(24), // Діє 24 години
                    HttpOnly = true, // Захист від XSS
                    IsEssential = true
                };

                Response.Cookies.Append(viewCookieName, "true", options);
            }

            ViewBag.Views = page.ViewsCount;
            var dto = PageMapper.ToDto(page);
            return View(dto);
        }


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