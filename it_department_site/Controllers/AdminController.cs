using System.Data;
using System.Text.Json;
using Dapper;
using it_department_site.Entities;
using it_department_site.Repositories.Interfaces;
using it_department_site.Services;
using it_department_site.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace it_department_site.Controllers
{
    [JwtAuthorize]
    public class AdminController : Controller
    {
        private readonly IPageService _pageService;
        private readonly IMenuService _menuService;
        private readonly ISiteSettingsService _settingsService;
        private readonly IPostRepository _postRepository;
        private readonly IDbConnection _db;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IPageService pageService,
            IMenuService menuService,
            ISiteSettingsService settingsService,
            IPostRepository postRepository,
            IDbConnection db,
            ILogger<AdminController> logger)
        {
            _pageService = pageService;
            _menuService = menuService;
            _settingsService = settingsService;
            _postRepository = postRepository;
            _db = db;
            _logger = logger;
        }

        // =========================
        // DASHBOARD
        // =========================
        public IActionResult Index()
        {
            return View();
        }

        // =========================
        // SITE SETTINGS
        // =========================
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

        // =========================
        // USERS (DAPPER)
        // =========================
        public async Task<IActionResult> Users()
        {
            var reader = await _db.ExecuteReaderAsync("SELECT * FROM users ORDER BY id");
            var dt = new DataTable();
            dt.Load(reader);

            return PartialView("_UsersTable", dt);
        }

        // =========================
        // PAGES
        // =========================
        public async Task<IActionResult> Pages(int page = 1)
        {
            int pageSize = 5;
            var pages = await _pageService.GetPagedAsync(page, pageSize);
            var totalPages = await _pageService.GetTotalAsync(pageSize);

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            return PartialView("_PagesTable", pages);
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
                await _pageService.UpdatePageAsync(page);

            if (model.ParentMenuItemId.HasValue)
                await _menuService.LinkPageToMenuAsync(page.Id, model.ParentMenuItemId, page.Title);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePage(int id)
        {
            await _pageService.DeletePageAsync(id);
            return RedirectToAction(nameof(Index));
        }

// =========================
// POSTS
// =========================

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

// Допоміжний метод для збереження картинки
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




        // =========================
        // MENU
        // =========================
        public async Task<IActionResult> MenuItems()
        {
            ViewBag.Pages = await _pageService.GetAllPagesAsync();
            return PartialView("_MenuItemsTable", await _menuService.GetAllMenuItemsAsync());
        }

        public async Task<IActionResult> CreateMenuItem()
        {
            ViewBag.Pages = await _pageService.GetAllPagesAsync();
            ViewBag.MenuItems = await _menuService.GetAllMenuItemsAsync();
            return View("EditMenuItem", new MenuItem());
        }

        public async Task<IActionResult> EditMenuItem(int id)
        {
            ViewBag.Pages = await _pageService.GetAllPagesAsync();
            ViewBag.MenuItems = await _menuService.GetAllMenuItemsAsync();
            return View(await _menuService.GetMenuItemByIdAsync(id));
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

        // UploadImage метод залишаємо (він вже async у твоєму коді був, просто скопіюй якщо треба, або лиши як є)
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

    }
    
}
