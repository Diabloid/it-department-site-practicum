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
        private readonly IUserService _userService;

        public AdminController(
            IPageService pageService,
            IMenuService menuService,
            IDbConnection db, // ✅ Інжектимо з'єднання
            ILogger<AdminController> logger,
            ISiteSettingsService settingsService,
            IPartnerService partnerService,
            IPostRepository postRepository,
            IGraduateService graduateService,
            IUserService userService
            )
        {
            _pageService = pageService;
            _menuService = menuService;
            _db = db;
            _logger = logger;
            _settingsService = settingsService;
            _partnerService = partnerService;
            _postRepository = postRepository;
            _graduateService = graduateService;
            _userService = userService;
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
                graduate.PhotoUrl = await SaveImage(photoFile, "graduates");

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
                graduate.PhotoUrl = await SaveImage(photoFile, "graduates");

            await _graduateService.UpdateAsync(graduate.Id, graduate);
            return await Graduates();
        }

        public async Task<IActionResult> DeleteGraduate(int id)
        {
            await _graduateService.DeleteAsync(id);
            return await Graduates();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllAsync();
            _logger.LogInformation("Admin requested users list, total {UserCount} users", users.Count());
            return PartialView("_UsersTable", users);
        }

        public async Task<IActionResult> Pages(int page = 1)
        {
            int pageSize = 5;
            var pages = await _pageService.GetPagedAsync(page, pageSize);
            var totalPages = await _pageService.GetTotalAsync(pageSize);

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            _logger.LogInformation("Admin requested pages list, page {PageNumber} of {TotalPages}", page, totalPages);
            return PartialView("_PagesTable", pages);
        }

        public async Task<IActionResult> MenuItems()
        {
            var menuItems = await _menuService.GetAllMenuItemsAsync();
            ViewBag.Pages = await _pageService.GetAllPagesAsync();
            _logger.LogInformation("Admin requested menu items list");
            return PartialView("_MenuItemsTable", menuItems);
        }

        public IActionResult Create()
        {
            _logger.LogInformation("Admin opened create page form");
            return View("CreatePage");
        }

        public async Task<IActionResult> PageEditor(int? id)
        {
            ViewBag.MenuItems = await _menuService.GetAllMenuItemsAsync();

            if (id == null)
            {
                _logger.LogInformation("Admin opened create page form");
                var layoutType = HttpContext.Session.GetString("SelectedLayoutType") ?? "no-sidebar";
                return View(new PageEditorViewModel { LayoutType = layoutType });
            }

            var page = await _pageService.GetPageByIdAsync(id.Value);
            if (page == null)
            {
                _logger.LogWarning("Page not found with ID {PageId}", id.Value);
                return NotFound();
            }

            return View(new PageEditorViewModel
            {
                Id = page.Id,
                Title = page.Title ?? "",
                Slug = page.Slug ?? "",
                Status = page.Status.ToString(),
                LayoutType = page.LayoutType ?? "",
                ContentSections = page.Content ?? new ContentSections(),
                CreatedAt = page.CreatedAt,
                UpdatedAt = page.UpdatedAt,
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
                LayoutType = model.LayoutType ?? "",
                CreatedAt = model.CreatedAt ?? DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            if (model.Id == null)
            {
                page.Id = await _pageService.CreatePageAsync(page);
                _logger.LogInformation("Page created with ID {PageId}", page.Id);
            }
            else
            {
                _logger.LogInformation("Page updated with ID {PageId}", page.Id);
                await _pageService.UpdatePageAsync(page);
            }

            if (model.ParentMenuItemId.HasValue)
            {
                _logger.LogInformation("Linking page ID {PageId} to menu item ID {MenuItemId}", page.Id, model.ParentMenuItemId.Value);
                await _menuService.LinkPageToMenuAsync(page.Id, model.ParentMenuItemId, page.Title);
            }
            _logger.LogInformation("Redirecting to pages list after saving page with ID {PageId}", page.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePage(int id)
        {
            _logger.LogInformation("Admin requested to delete page with ID {PageId}", id);
            await _pageService.DeletePageAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult SaveTemplateSelection([FromBody] TemplateSelectionModel model)
        {
            if (!string.IsNullOrEmpty(model.LayoutType))
            {
                _logger.LogInformation("Admin selected layout type: {LayoutType}", model.LayoutType);
                HttpContext.Session.SetString("SelectedLayoutType", model.LayoutType);
                return Ok();
            }

            return BadRequest();
        }

        public class TemplateSelectionModel
        {
            public string? LayoutType { get; set; }
        }

        public async Task<IActionResult> AllPages(int page = 1)
        {
            int pageSize = 3;
            var pages = await _pageService.GetPagedAsync(page, pageSize);
            var totalPages = await _pageService.GetTotalAsync(pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            _logger.LogInformation("Admin requested all pages list, page {PageNumber} of {TotalPages}", page, totalPages);
            return View(pages);
        }

        public async Task<IActionResult> CreateMenuItem()
        {
            var pages = await _pageService.GetAllPagesAsync();
            ViewBag.Pages = pages.Select(PageMapper.ToDto).ToList();
            ViewBag.MenuItems = await _menuService.GetAllMenuItemsAsync();
            _logger.LogInformation("Admin opened create menu item form");
            return View("EditMenuItem", new MenuItem());
        }

        public async Task<IActionResult> EditMenuItem(int id)
        {
            var item = await _menuService.GetMenuItemByIdAsync(id);
            var pages = await _pageService.GetAllPagesAsync();
            ViewBag.Pages = pages.Select(PageMapper.ToDto).ToList();
            ViewBag.MenuItems = await _menuService.GetAllMenuItemsAsync();
            _logger.LogInformation("Admin opened edit form for menu item with ID {MenuItemId}", id);
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMenuItem(MenuItem item)
        {
            if (item.Id == 0)
            {
                _logger.LogInformation("Creating new menu item: {MenuItemName}", item.Name);
                await _menuService.CreateMenuItemAsync(item);
            }
            else
            {
                _logger.LogInformation("Updating menu item: {MenuItemName}", item.Name);
                await _menuService.UpdateMenuItemAsync(item);
            }

            _logger.LogInformation("Menu item saved successfully: {MenuItemName}", item.Name);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            await _menuService.DeleteMenuItemAsync(id);
            _logger.LogInformation("Deleted menu item with ID {MenuItemId}", id);
            return RedirectToAction("Index", new { section = "menuItems" });
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Admin attempted to upload an empty file");
                return BadRequest("File is empty");
            }
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                _logger.LogInformation("Uploads directory does not exist, creating at {UploadsPath}", uploadsPath);
                Directory.CreateDirectory(uploadsPath);
            }
            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            _logger.LogInformation("Admin uploaded image: {FileName}", fileName);
            return Json(new { data = new[] { new { url = $"/uploads/{fileName}" } } });
        }

        public async Task<IActionResult> Partners()
        {
            var partners = await _partnerService.GetAllPartnersAsync();
            _logger.LogInformation("Admin requested partners list, total {PartnerCount} partners", partners.Count());
            return PartialView("_Partners", partners.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> UploadPartner(IFormFile file, string name, string link)
        {
            await _partnerService.AddPartnerAsync(file, name, link);
            _logger.LogInformation("Admin uploaded new partner: {PartnerName}", name);
            return RedirectToAction("Index", new { section = "Partners" });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePartner(int id)
        {
            await _partnerService.DeletePartnerAsync(id);
            _logger.LogInformation("Admin deleted partner with ID {PartnerId}", id);
            return RedirectToAction("Index", new { section = "Partners" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePartnerOrder([FromBody] List<int> newOrder)
        {
            await _partnerService.UpdatePartnerOrderAsync(newOrder);
            _logger.LogInformation("Admin updated partner order");
            return Ok();
        }

        public async Task<IActionResult> Posts()
        {
            var posts = await _postRepository.GetAllAsync();
            _logger.LogInformation("Admin requested posts list, total {PostCount} posts", posts.Count());
            return PartialView("_PostsTable", posts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(Post post, IFormFile? thumbnailFile)
        {
            ModelState.Remove("Author");
            ModelState.Remove("Thumbnail"); // Видаляємо, бо заповнимо вручну

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Admin attempted to create a post with invalid data");
                return PartialView("_CreatePost", post);
            }

            try
            {
                if (thumbnailFile != null)
                {
                    _logger.LogInformation("Saving thumbnail for new post: {FileName}", thumbnailFile.FileName);
                    post.Thumbnail = await SaveImage(thumbnailFile, "posts");
                }

                post.AuthorId = 1;
                post.CreatedAt = DateTime.UtcNow;
                post.UpdatedAt = DateTime.UtcNow;

                await _postRepository.CreateAsync(post);
                _logger.LogInformation("Post created with ID {PostId}", post.Id);
                return await Posts();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a post");
                _logger.LogWarning("Admin attempted to create a post with invalid data");
                return StatusCode(500, "Помилка сервера");
            }
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                _logger.LogWarning("Admin attempted to edit a post with ID {PostId} that does not exist", id);
                return NotFound();
            }
            _logger.LogInformation("Admin opened edit form for post with ID {PostId}", id);
            return PartialView("_EditPost", post);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Post post, IFormFile? thumbnailFile)
        {
            ModelState.Remove("Author");
            ModelState.Remove("Thumbnail");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Admin attempted to edit a post with ID {PostId} that has invalid data", post.Id);
                return PartialView("_EditPost", post);
            }

            try
            {
                if (thumbnailFile != null)
                {
                    // Якщо завантажили нове фото — зберігаємо його
                    _logger.LogInformation("Saving new thumbnail for post with ID {PostId}: {FileName}", post.Id, thumbnailFile.FileName);
                    post.Thumbnail = await SaveImage(thumbnailFile, "posts");
                }

                post.UpdatedAt = DateTime.UtcNow;
                await _postRepository.UpdateAsync(post);
                _logger.LogInformation("Post with ID {PostId} updated successfully", post.Id);
                return await Posts();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while editing a post");
                return StatusCode(500, "Помилка сервера");
            }
        }

        private async Task<string> SaveImage(IFormFile file, string folderName)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);

            if (!Directory.Exists(rootPath))
            {

                Directory.CreateDirectory(rootPath);
                _logger.LogInformation("Created directory for uploads at {RootPath}", rootPath);
            }

            var filePath = Path.Combine(rootPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                _logger.LogInformation("Saving file to {FilePath}", filePath);
                await file.CopyToAsync(stream);
            }
            _logger.LogInformation("File saved successfully: {FileName}", fileName);
            return $"/uploads/{folderName}/{fileName}";
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _postRepository.DeleteAsync(id);
            _logger.LogInformation("Admin deleted post with ID {PostId}", id);
            return await Posts(); // 🔥 Оновлюємо таблицю
        }
    }
}
