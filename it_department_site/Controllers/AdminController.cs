using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using it_department_site.Models;
using it_department_site.Services;
using it_department_site.Database;
using dbhelper;
using it_department_site.Filters;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace it_department_site.Controllers
{
    [JwtAuthorize]
    public class AdminController : Controller
    {
        private readonly IPageService _pageService;
        private readonly IMenuService _menuService;
        private readonly ILogger<AdminController> _logger;
        private readonly Dbhelper _dbHelper;

        public AdminController(
            IPageService pageService,
            IMenuService menuService,
            Dbhelper dbHelper,
            ILogger<AdminController> logger)
        {
            _pageService = pageService;
            _menuService = menuService;
            _dbHelper = dbHelper;
            _logger = logger;
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
        public IActionResult Pages()
        {
            var pages = _pageService.GetAllPages();
            _logger.LogInformation("Адмін переглянув {Count} сторінок", pages.Count);
            return PartialView("_PagesTable", pages);
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
                    UpdatedAt = page.UpdatedAt
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
                CreatedAt = (DateTime)(model.CreatedAt != default ? model.CreatedAt : DateTime.Now),
                UpdatedAt = DateTime.Now
            };

            if (model.Id == null)
            {
                _pageService.CreatePage(page);
                _logger.LogInformation("Створено сторінку: {Title}", page.Title);
            }
            else
            {
                _pageService.UpdatePage(page);
                _logger.LogInformation("Оновлено сторінку ID {Id}", page.Id);
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

    }
}
