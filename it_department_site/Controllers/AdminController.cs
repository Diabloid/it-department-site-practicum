using Microsoft.AspNetCore.Authorization;

namespace it_department_site.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IPageService _pageService;
        private readonly IMenuService _menuService;
        private readonly ILogger<AdminController> _logger;
        private readonly ISiteSettingsService _settingsService;
        private readonly IDbConnection _db; // ✅ Замість Dbhelper
        private readonly IPartnerService _partnerService;
        private readonly IPostRepository _postRepository;
        private readonly IGraduateService _graduateService;

        public AdminController(
            IPageService pageService,
            IMenuService menuService,
            IDbConnection db, // ✅ Інжектимо з'єднання
            ILogger<AdminController> logger,
            ISiteSettingsService settingsService,
            IPartnerService partnerService,
            IPostRepository postRepository,
            IGraduateService graduateService)
        {
            _pageService = pageService;
            _menuService = menuService;
            _db = db;
            _logger = logger;
            _settingsService = settingsService;
            _partnerService = partnerService;
            _postRepository = postRepository;
            _graduateService = graduateService;
        }

        public async Task<IActionResult> LayoutSettings()
        {
            var settings = await _settingsService.GetAsync();
            return View("EditLayout", settings);
        }

        [HttpPost]
        public async Task<IActionResult> LayoutSettings(SiteSettings model)
        {
            if (string.IsNullOrWhiteSpace(model.SiteTitle))
            {
                TempData["Error"] = "Назва сайту не може бути порожньою!";
                return RedirectToAction(nameof(LayoutSettings));
            }

            var current = await _settingsService.GetAsync();
            current.SiteTitle = model.SiteTitle;
            current.FooterHtml = model.FooterHtml;

            await _settingsService.SaveAsync(current);

            TempData["Success"] = "Зміни збережено!";
            return RedirectToAction(nameof(LayoutSettings));
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Teacher()
        {
            return PartialView("_TeacherTable");
        }
        public IActionResult CreateGraduate()
        {
            return PartialView("_CreateGraduate", new Graduate());
        }
        public async Task<IActionResult> Graduates()
        {
            var graduates = await _graduateService.GetAllAsync();
            return PartialView("_GraduatesTable", graduates);
        }
        public async Task<IActionResult> EditGraduate(int id)
        {
            var graduate = await _graduateService.GetByIdAsync(id);
            if (graduate == null) return NotFound();
            return PartialView("_EditGraduate", graduate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGraduate(Graduate graduate, IFormFile? photoFile)
        {
            ModelState.Remove("PhotoUrl");
            if (!ModelState.IsValid) return PartialView("_CreateGraduate", graduate);

            if (photoFile != null)
                graduate.PhotoUrl = await SaveGraduateImage(photoFile);

            await _graduateService.CreateAsync(graduate);
            return await Graduates();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGraduate(Graduate graduate, IFormFile? photoFile)
        {
            ModelState.Remove("PhotoUrl");
            if (!ModelState.IsValid) return PartialView("_EditGraduate", graduate);

            if (photoFile != null)
                graduate.PhotoUrl = await SaveGraduateImage(photoFile);

            await _graduateService.UpdateAsync(graduate.Id, graduate);
            return await Graduates();
        }


        public async Task<IActionResult> DeleteGraduate(int id)
        {
            await _graduateService.DeleteAsync(id);
            return await Graduates();
        }

        private async Task<string> SaveGraduateImage(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "graduates");
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
            var filePath = Path.Combine(rootPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);
            return "/uploads/graduates/" + fileName;
        }
        public async Task<IActionResult> Users()
        {
            var reader = await _db.ExecuteReaderAsync("SELECT * FROM users ORDER BY id");
            var dt = new DataTable();
            dt.Load(reader);

            return PartialView("_UsersTable", dt);
        }

        public async Task<IActionResult> Pages(int page = 1)
        {
            int pageSize = 5;
            var pages = await _pageService.GetPagedAsync(page, pageSize);
            var totalPages = await _pageService.GetTotalAsync(pageSize);

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            return PartialView("_PagesTable", pages);
        }

        public async Task<IActionResult> MenuItems()
        {
            var menuItems = await _menuService.GetAllMenuItemsAsync();
            ViewBag.Pages = await _pageService.GetAllPagesAsync();

            return PartialView("_MenuItemsTable", menuItems);
        }

        public IActionResult Create()
        {
            return View("CreatePage");
        }

        public async Task<IActionResult> PageEditor(int? id)
        {
            ViewBag.MenuItems = await _menuService.GetAllMenuItemsAsync();

            if (id == null)
            {
                var layoutType = HttpContext.Session.GetString("SelectedLayoutType") ?? "no-sidebar";
                return View(new PageEditorViewModel { LayoutType = layoutType });
            }

            var page = await _pageService.GetPageByIdAsync(id.Value);
            if (page == null) return NotFound();

            return View(new PageEditorViewModel
            {
                Id = page.Id,
                Title = page.Title,
                Slug = page.Slug,
                Status = page.Status.ToString(),
                LayoutType = page.LayoutType,
                ContentSections = page.Content,
                CreatedAt = page.CreatedAt,
                UpdatedAt = page.UpdatedAt,
                AllowComments = page.AllowComments
            });
        }

        [HttpPost]
        public async Task<IActionResult> SavePage(PageEditorViewModel model)
        {
            var page = new Page
            {
                Id = model.Id ?? 0,
                Title = model.Title,
                Slug = model.Slug,
                Content = model.ContentSections,
                Status = Enum.TryParse(model.Status, out PageStatus status) ? status : PageStatus.Draft,
                LayoutType = model.LayoutType,
                AllowComments = model.AllowComments,
                CreatedAt = model.CreatedAt ?? DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            if (model.Id == null)
                page.Id = await _pageService.CreatePageAsync(page);
            else
            {
                await _pageService.UpdatePageAsync(page);
            }

            if (model.ParentMenuItemId.HasValue)
            {
                await _menuService.LinkPageToMenuAsync(page.Id, model.ParentMenuItemId, page.Title);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePage(int id)
        {
            await _pageService.DeletePageAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult SaveTemplateSelection([FromBody] TemplateSelectionModel model)
        {
            if (!string.IsNullOrEmpty(model.LayoutType))
            {
                HttpContext.Session.SetString("SelectedLayoutType", model.LayoutType);
                return Ok();
            }

            return BadRequest();
        }

        public class TemplateSelectionModel
        {
            public string LayoutType { get; set; }
        }

        public async Task<IActionResult> AllPages(int page = 1)
        {
            int pageSize = 3;
            var pages = await _pageService.GetPagedAsync(page, pageSize);
            var totalPages = await _pageService.GetTotalAsync(pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pages);
        }

        public async Task<IActionResult> CreateMenuItem()
        {
            var pages = await _pageService.GetAllPagesAsync();
            ViewBag.Pages = pages.Select(PageMapper.ToDto).ToList();
            ViewBag.MenuItems = await _menuService.GetAllMenuItemsAsync();
            return View("EditMenuItem", new MenuItem());
        }

        public async Task<IActionResult> EditMenuItem(int id)
        {
            var item = await _menuService.GetMenuItemByIdAsync(id);
            var pages = await _pageService.GetAllPagesAsync();
            ViewBag.Pages = pages.Select(PageMapper.ToDto).ToList();
            ViewBag.MenuItems = await _menuService.GetAllMenuItemsAsync();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMenuItem(MenuItem item)
        {
            if (item.Id == 0)
                await _menuService.CreateMenuItemAsync(item);
            else
                await _menuService.UpdateMenuItemAsync(item);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            await _menuService.DeleteMenuItemAsync(id);
            return RedirectToAction("Index", new { section = "menuItems" });
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("File is empty");
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);
            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Json(new { data = new[] { new { url = $"/uploads/{fileName}" } } });
        }

        public async Task<IActionResult> Partners()
        {
            var partners = await _partnerService.GetAllPartnersAsync();
            return PartialView("_Partners", partners.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> UploadPartner(IFormFile file, string name, string link)
        {
            await _partnerService.AddPartnerAsync(file, name, link);
            return RedirectToAction("Index", new { section = "Partners" });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePartner(int id)
        {
            await _partnerService.DeletePartnerAsync(id);
            return RedirectToAction("Index", new { section = "Partners" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePartnerOrder([FromBody] List<int> newOrder)
        {
            await _partnerService.UpdatePartnerOrderAsync(newOrder);
            return Ok();
        }

        public async Task<IActionResult> Posts()
        {
            var posts = await _postRepository.GetAllAsync();
            return PartialView("_PostsTable", posts);
        }

        public IActionResult CreatePost()
        {
            return PartialView("_CreatePost", new Post());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(Post post, IFormFile? thumbnailFile)
        {
            ModelState.Remove("Author");
            ModelState.Remove("Thumbnail"); // Видаляємо, бо заповнимо вручну

            if (!ModelState.IsValid) return PartialView("_CreatePost", post);

            try
            {
                if (thumbnailFile != null)
                {
                    post.Thumbnail = await SaveImage(thumbnailFile);
                }

                post.AuthorId = 1;
                post.CreatedAt = DateTime.UtcNow;
                post.UpdatedAt = DateTime.UtcNow;

                await _postRepository.CreateAsync(post);
                return await Posts();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні");
                return StatusCode(500, "Помилка сервера");
            }
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null) return NotFound();

            return PartialView("_EditPost", post);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Post post, IFormFile? thumbnailFile)
        {
            ModelState.Remove("Author");
            ModelState.Remove("Thumbnail");

            if (!ModelState.IsValid) return PartialView("_EditPost", post);

            try
            {
                if (thumbnailFile != null)
                {
                    // Якщо завантажили нове фото — зберігаємо його
                    post.Thumbnail = await SaveImage(thumbnailFile);
                }

                post.UpdatedAt = DateTime.UtcNow;
                await _postRepository.UpdateAsync(post);
                return await Posts();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при редагуванні");
                return StatusCode(500, "Помилка сервера");
            }
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posts");

            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

            var filePath = Path.Combine(rootPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/posts/" + fileName;
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _postRepository.DeleteAsync(id);

            return await Posts(); // 🔥 Оновлюємо таблицю
        }
    }
}
