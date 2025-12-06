using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using it_department_site.Models;
using it_department_site.Services;
using it_department_site.Database;
using dbhelper;
using it_department_site.Filters;
using System.Text.Json;
using System.Text.Encodings.Web;
using it_department_site.Repositories;

namespace it_department_site.Controllers
{
    [JwtAuthorize]
    public class AdminController : Controller
    {
        private readonly IPageService _pageService;
        private readonly IMenuService _menuService;
        private readonly ILogger<AdminController> _logger;
        private readonly Dbhelper _dbHelper;
        private readonly ISiteSettingsService _settingsService;

        public AdminController(
            IPageService pageService,
            IMenuService menuService,
            Dbhelper dbHelper,
            ILogger<AdminController> logger,
            ISiteSettingsService settingsService)
        {
            _pageService = pageService;
            _menuService = menuService;
            _dbHelper = dbHelper;
            _logger = logger;
            _settingsService = settingsService;
        }
        public IActionResult LayoutSettings()
        {
            var settings = _settingsService.Get();
            return View("EditLayout", settings);
        }
        [HttpPost]
        public IActionResult LayoutSettings(SiteSettings model)
        {
            if (string.IsNullOrWhiteSpace(model.SiteTitle))
            {
                TempData["Error"] = "Назва сайту не може бути порожньою!";
                return RedirectToAction("LayoutSettings");
            }

            var current = _settingsService.Get();
            current.SiteTitle = model.SiteTitle;
            current.FooterHtml = model.FooterHtml;

            _settingsService.Save(current);

            TempData["Success"] = "Зміни збережено!";
            return RedirectToAction("LayoutSettings");
        }
        // Головна сторінка адмінки
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Users()
        {
            var dt = _dbHelper.ExecuteQuery("SELECT * FROM users ORDER BY id");
            return PartialView("_UsersTable", dt);
        }
        // Список усіх сторінок
        public IActionResult Pages(int page = 1)
        {
            int pageSize = 5;
            var pages = _pageService.GetPaged(page, pageSize);
            var totalPages = _pageService.GetTotal(pageSize);

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            return PartialView("_PagesTable", pages);
        }
        public IActionResult MenuItems()
        {
            var menuItems = _menuService.GetAllMenuItems();
            ViewBag.Pages = _pageService.GetAllPages(); // додаємо всі сторінки

            return PartialView("_MenuItemsTable", menuItems);
        }

        public IActionResult Create()
        {
            return View("CreatePage");
        }

        // Редактор сторінки (створення або редагування)
        public IActionResult PageEditor(int? id)
        {
            PageEditorViewModel model;
            ViewBag.MenuItems = _menuService.GetAllMenuItems();

            if (id == null)
            {
                var layoutType = HttpContext.Session.GetString("SelectedLayoutType") ?? "no-sidebar";

                model = new PageEditorViewModel
                {
                    LayoutType = layoutType
                };
            }
            else
            {
                var page = _pageService.GetPageById(id.Value);
                if (page == null)
                {
                    _logger.LogWarning($"Сторінка з ID {id.Value} не знайдена");
                    return NotFound();
                }

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

        // POST: Збереження сторінки
        public IActionResult SavePage(PageEditorViewModel model)
        {
            var page = new Page
            {
                Id = model.Id ?? 0,
                Title = model.Title,
                Slug = model.Slug,
                Content = model.ContentSections,
                Status = Enum.TryParse(model.Status, out PageStatus status) ? status : PageStatus.Draft,
                LayoutType = model.LayoutType,
                AllowComments = model.AllowComments, // ← Додай це!
                CreatedAt = (DateTime)(model.CreatedAt != default ? model.CreatedAt : DateTime.Now),
                UpdatedAt = DateTime.Now
            };

            if (model.Id == null)
            {
                int newPageId = _pageService.CreatePage(page);
                page.Id = newPageId;
                _logger.LogInformation("Створено сторінку: {Title}", page.Title);
            }
            else
            {
                _pageService.UpdatePage(page);
                _logger.LogInformation("Оновлено сторінку ID {Id}", page.Id);
            }

            if (model.ParentMenuItemId.HasValue)
            {
                _menuService.LinkPageToMenu(page.Id, model.ParentMenuItemId, page.Title);
            }

            return RedirectToAction("Index");
        }

        // POST: Видалення сторінки
        [HttpPost]
        public IActionResult DeletePage(int id)
        {
            _pageService.DeletePage(id);
            _logger.LogWarning("Сторінку з ID {Id} видалено", id);
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

        public IActionResult AllPages(int page = 1)
        {
            int pageSize = 3;
            var pages = _pageService.GetPaged(page, pageSize);
            var totalPages = _pageService.GetTotal(pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pages);
        }
        public IActionResult CreateMenuItem()
        {
            ViewBag.Pages = _pageService.GetAllPages(); // для вибору сторінки
            ViewBag.MenuItems = _menuService.GetAllMenuItems(); // для вибору батька
            return View("EditMenuItem", new MenuItem());
        }

        public IActionResult EditMenuItem(int id)
        {
            var item = _menuService.GetMenuItemById(id);
            ViewBag.Pages = _pageService.GetAllPages();
            ViewBag.MenuItems = _menuService.GetAllMenuItems();
            return View(item);
        }


        [HttpPost]
        public IActionResult SaveMenuItem(MenuItem item)
        {
            if (item.Id == 0)
                _menuService.CreateMenuItem(item);
            else
                _menuService.UpdateMenuItem(item);

            return RedirectToAction("Index", new { section = "menuItems" });
        }

        [HttpPost]
        public IActionResult DeleteMenuItem(int id)
        {
            _menuService.DeleteMenuItem(id);
            return RedirectToAction("Index", new { section = "menuItems" });
        }
        public IActionResult GetMenuItem(int id)
        {
            var item = _menuService.GetMenuItemById(id);
            return Json(item);
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл порожній");

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Повертаємо шлях, який буде використовуватись у <img src="...">
            var fileUrl = $"/uploads/{fileName}";
            return Json(new { data = new[] { new { url = fileUrl } } });
        }
    }
}
