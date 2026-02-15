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

        public AdminController(
            IPageService pageService,
            IMenuService menuService,
            IDbConnection db, // ✅ Інжектимо з'єднання
            ILogger<AdminController> logger,
            ISiteSettingsService settingsService)
        {
            _pageService = pageService;
            _menuService = menuService;
            _db = db;
            _logger = logger;
            _settingsService = settingsService;
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

        public IActionResult Partners()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "partners");
            var orderFilePath = Path.Combine(folderPath, "order.json");

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var dirInfo = new DirectoryInfo(folderPath);

            var allFiles = dirInfo.GetFiles()
                                    .Where(f => !f.Name.EndsWith("order.json"))
                                    .OrderBy(f => f.CreationTime)
                                    .Select(f => "/images/partners/" + f.Name)
                                    .ToList();

            List<string> orderedFiles = new List<string>();

            if (System.IO.File.Exists(orderFilePath))
            {
                try
                {
                    var json = System.IO.File.ReadAllText(orderFilePath);
                    var savedOrder = JsonSerializer.Deserialize<List<string>>(json);

                    orderedFiles = savedOrder.Where(f => allFiles.Contains(f)).ToList();
                }
                catch { }
            }

            foreach (var file in allFiles)
            {
                if (!orderedFiles.Contains(file))
                {
                    orderedFiles.Add(file);
                }
            }

            return PartialView("_Partners", orderedFiles);
        }

        [HttpPost]
        public async Task<IActionResult> UploadPartner(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "partners");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return RedirectToAction("Index", new { section = "Partners" });
        }

        [HttpPost]
        public IActionResult DeletePartner(string fileName)
        {
            var name = Path.GetFileName(fileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "partners", name);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToAction("Index", new { section = "Partners" });
        }
        [HttpPost]
        public IActionResult UpdatePartnerOrder([FromBody] List<string> order)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "partners");
            var orderFilePath = Path.Combine(folderPath, "order.json");

            // Зберігаємо список імен файлів у JSON
            var json = JsonSerializer.Serialize(order);
            System.IO.File.WriteAllText(orderFilePath, json);

            return Ok();
        }
    }
}