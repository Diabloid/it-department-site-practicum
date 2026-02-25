namespace it_department_site.Controllers
{
    [JwtAuthorize]
    public class AdminController : Controller
    {
        private readonly IPageService _pageService;
        private readonly IMenuService _menuService;
        private readonly ILogger<AdminController> _logger;
        private readonly ISiteSettingsService _settingsService;
        private readonly IDbConnection _db; // ✅ Замість Dbhelper
        private readonly IPartnerService _partnerService;
        public AdminController(
            IPageService pageService,
            IMenuService menuService,
            IDbConnection db, // ✅ Інжектимо з'єднання
            ILogger<AdminController> logger,
            ISiteSettingsService settingsService,
            IPartnerService partnerService)
        {
            _pageService = pageService;
            _menuService = menuService;
            _db = db;
            _logger = logger;
            _settingsService = settingsService;
            _partnerService = partnerService;
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
                return RedirectToAction("LayoutSettings");
            }

            var current = await _settingsService.GetAsync();
            current.SiteTitle = model.SiteTitle;
            current.FooterHtml = model.FooterHtml;

            await _settingsService.SaveAsync(current);

            TempData["Success"] = "Зміни збережено!";
            return RedirectToAction("LayoutSettings");
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            // ✅ Dapper замість dbHelper.ExecuteQuery
            // Оскільки View очікує DataTable (старий код), краще поки передати список,
            // АЛЕ щоб не ламати View "_UsersTable", зробимо трюк з IDataReader або змінимо View.
            // Найпростіше: отримати список і передати його, але View "_UsersTable" працює з DataTable.
            // Щоб не переписувати View, зробимо Load:

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
            PageEditorViewModel model;
            ViewBag.MenuItems = await _menuService.GetAllMenuItemsAsync();

            if (id == null)
            {
                var layoutType = HttpContext.Session.GetString("SelectedLayoutType") ?? "no-sidebar";
                model = new PageEditorViewModel { LayoutType = layoutType };
            }
            else
            {
                var page = await _pageService.GetPageByIdAsync(id.Value);
                if (page == null) return NotFound();

                model = new PageEditorViewModel
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
                };
            }

            return View(model);
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
                CreatedAt = (DateTime)(model.CreatedAt != default ? model.CreatedAt : DateTime.Now),
                UpdatedAt = DateTime.Now
            };

            if (model.Id == null)
            {
                int newPageId = await _pageService.CreatePageAsync(page);
                page.Id = newPageId;
            }
            else
            {
                await _pageService.UpdatePageAsync(page);
            }

            if (model.ParentMenuItemId.HasValue)
            {
                await _menuService.LinkPageToMenuAsync(page.Id, model.ParentMenuItemId, page.Title);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePage(int id)
        {
            await _pageService.DeletePageAsync(id);
            return RedirectToAction("Index");
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
            ViewBag.Pages = await _pageService.GetAllPagesAsync();
            ViewBag.MenuItems = await _menuService.GetAllMenuItemsAsync();
            return View("EditMenuItem", new MenuItem());
        }

        public async Task<IActionResult> EditMenuItem(int id)
        {
            var item = await _menuService.GetMenuItemByIdAsync(id);
            ViewBag.Pages = await _pageService.GetAllPagesAsync();
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

            return RedirectToAction("Index", new { section = "menuItems" });
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
            using (var stream = new FileStream(filePath, FileMode.Create)) { await file.CopyToAsync(stream); }
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
    }
}