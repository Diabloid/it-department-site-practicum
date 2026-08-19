using it_department_site.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IPageService _pageService;
        private readonly IMenuService _menuService;
        private readonly ILogger<AdminController> _logger;
        private readonly ISiteSettingsService _settingsService;
        private readonly IPartnerService _partnerService;
        private readonly IPostService _postService;
        private readonly IGraduateService _graduateService;
        private readonly IEmployeeService _employeeService;
        private readonly IGoogleDriveService _driveService;
        private readonly GoogleDriveOptions _driveOptions;
        private readonly IMainSliderService _mainSliderService;
        private readonly IDisciplineService _disciplineService;
        private readonly IUserService _userService;
        private readonly IFileStorageService _fileStorageService;

        public AdminController(
            IPageService pageService,
            IMenuService menuService,
            ILogger<AdminController> logger,
            ISiteSettingsService settingsService,
            IPartnerService partnerService,
            IPostService postService,
            IGraduateService graduateService,
            IEmployeeService employeeService,
            IGoogleDriveService driveService,
            Microsoft.Extensions.Options.IOptions<GoogleDriveOptions> driveOptions,
            IMainSliderService mainSliderService,
            IDisciplineService disciplineService,
            IUserService userService,
            IFileStorageService fileStorageService)
        {
            _pageService = pageService;
            _menuService = menuService;
            _logger = logger;
            _settingsService = settingsService;
            _partnerService = partnerService;
            _postService = postService;
            _graduateService = graduateService;
            _employeeService = employeeService;
            _driveService = driveService;
            _driveOptions = driveOptions.Value;
      	    _mainSliderService = mainSliderService;
            _disciplineService = disciplineService;
            _userService = userService;
            _fileStorageService = fileStorageService;
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
                graduate.PhotoUrl = await _fileStorageService.SaveImage(photoFile, "graduates");

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
                graduate.PhotoUrl = await _fileStorageService.SaveImage(photoFile, "graduates");

            await _graduateService.UpdateAsync(graduate.Id, graduate);
            return await Graduates();
        }

        public async Task<IActionResult> DeleteGraduate(int id)
        {
            await _graduateService.DeleteAsync(id);
            return await Graduates();
        }

        //===========================================
        public async Task<IActionResult> CreateEmployee()
        {
            ViewBag.AllDisciplines = await _disciplineService.GetAllAsync();
            return PartialView("_CreateEmployee", new Employee());
        }

        public async Task<IActionResult> Employees()
        {
            var employees = await _employeeService.GetAllAsync();
            var employeeList = employees.ToList();
            var counts = await _employeeService.GetCertificateCountsAsync(employeeList.Select(e => e.Id));
            ViewBag.CertificateCounts = counts;
            return PartialView("_EmployeesTable", employeeList);
        }

        public async Task<IActionResult> EditEmployee(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();

            ViewBag.DriveCertificateItems = await _employeeService.GetDriveCertificatePickerItemsAsync(id);
            ViewBag.AllDisciplines = await _disciplineService.GetAllAsync();
            return PartialView("_EditEmployee", employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(Employee employee, IFormFile? photoFile, int[]? selectedDisciplines)
        {
            ModelState.Remove("PhotoUrl");
            if (!ModelState.IsValid) return PartialView("_CreateEmployee", employee);

            if (photoFile != null)
                employee.PhotoUrl = await _fileStorageService.SaveImage(photoFile, "employees");
            else
                employee.PhotoUrl = "/images/banner_photo_2.jpg";

            employee.PatronymicName ??= string.Empty;
            employee.Email ??= string.Empty;
            employee.Specialization ??= string.Empty;
            employee.Bio ??= string.Empty;

            if (selectedDisciplines != null && selectedDisciplines.Length > 0)
            {
                var allDisciplines = await _disciplineService.GetAllAsync();
                employee.Disciplines = allDisciplines
                    .Where(d => selectedDisciplines.Contains(d.Id))
                    .ToList();
            }
            else
            {
                employee.Disciplines = new List<Discipline>();
            }

            await _employeeService.CreateAsync(employee);
            if (!string.IsNullOrWhiteSpace(employee.GoogleDriveFolderUrl))
                await _employeeService.ApplyDrivePhotoFromFolderAsync(employee.Id);
            return await Employees();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployee(
            Employee employee,
            IFormFile? photoFile,
            string[]? selectedCertificateFileIds,
            int[]? selectedDisciplines)
        {
            ModelState.Remove("PhotoUrl");
            if (!ModelState.IsValid)
            {
                ViewBag.DriveCertificateItems = await _employeeService.GetDriveCertificatePickerItemsAsync(employee.Id);
                ViewBag.AllDisciplines = await _disciplineService.GetAllAsync();
                return PartialView("_EditEmployee", employee);
            }

            if (selectedDisciplines != null && selectedDisciplines.Length > 0)
            {
                var allDisciplines = await _disciplineService.GetAllAsync();
                employee.Disciplines = allDisciplines
                    .Where(d => selectedDisciplines.Contains(d.Id))
                    .ToList();
            }
            else
            {
                employee.Disciplines = new List<Discipline>();
            }

            if (photoFile != null)
                employee.PhotoUrl = await _fileStorageService.SaveImage(photoFile, "employees");

            await _employeeService.UpdateAsync(employee.Id, employee);

            if (!string.IsNullOrWhiteSpace(employee.GoogleDriveFolderUrl))
            {
                if (photoFile == null)
                    await _employeeService.ApplyDrivePhotoFromFolderAsync(employee.Id);

                await _employeeService.SetSelectedDriveCertificatesAsync(
                    employee.Id,
                    selectedCertificateFileIds ?? Array.Empty<string>());
            }

            return await Employees();
        }

        [HttpGet]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> DriveFilePreview(int employeeId, string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId) ||
                !await _employeeService.IsFileInEmployeeDriveFolderAsync(employeeId, fileId))
            {
                return NotFound();
            }

            var preview = await _driveService.GetPreviewStreamAsync(fileId);
            if (preview == null)
                return NotFound();

            return File(preview.Value.Stream, preview.Value.ContentType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SyncDriveFolders()
        {
            var rootFolderId = _driveOptions.RootFolderId;
            var syncResult = await _employeeService.SyncDriveFoldersFromRootAsync(rootFolderId);
            ViewBag.DriveSyncResult = syncResult;
            return await Employees();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            await _employeeService.DeleteAsync(id);
            return await Employees();
        }


        public IActionResult CreateDiscipline()
        {
            return PartialView("_CreateDiscipline", new Discipline());
        }

        public async Task<IActionResult> Disciplines()
        {
            var disciplines = await _disciplineService.GetAllAsync();
            return PartialView("_DisciplinesTable", disciplines);
        }

        public async Task<IActionResult> EditDiscipline(int id)
        {
            var discipline = await _disciplineService.GetByIdAsync(id);
            if (discipline == null) return NotFound();
            return PartialView("_EditDiscipline", discipline);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDiscipline(Discipline discipline)
        {
            if (!ModelState.IsValid) return PartialView("_CreateDiscipline", discipline);

            await _disciplineService.CreateAsync(discipline);
            return await Disciplines();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDiscipline(Discipline discipline)
        {
            if (!ModelState.IsValid) return PartialView("_EditDiscipline", discipline);

            await _disciplineService.UpdateAsync(discipline.Id, discipline);
            return await Disciplines();
        }

        public async Task<IActionResult> DeleteDiscipline(int id)
        {
            await _disciplineService.DeleteAsync(id);
            return await Disciplines();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllAsync();
            return PartialView("_UsersTable", users);
        }

        public async Task<IActionResult> Pages(int page = 1)
        {
            int pageSize = 5;
            var pages = await _pageService.GetPagedAsync(page, pageSize);
            var totalPages = await _pageService.GetTotalPagesAsync(pageSize);

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
            if (page == null)
            {
                _logger.LogWarning("Page not found with ID {PageId}", id.Value);
                return NotFound();
            }

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
                CreatedAt = model.CreatedAt ?? DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            if (model.Id == null)
            {
                page.Id = await _pageService.CreatePageAsync(page);
            }
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
            _logger.LogInformation("Admin requested to delete page with ID {PageId}", id);
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
            public string? LayoutType { get; set; }
        }

        public async Task<IActionResult> AllPages(int page = 1)
        {
            int pageSize = 3;
            var pages = await _pageService.GetPagedAsync(page, pageSize);
            var totalPages = await _pageService.GetTotalPagesAsync(pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pages);
        }

        public async Task<IActionResult> CreateMenuItem()
        {
            var pages = await _pageService.GetAllPagesAsync();
            ViewBag.Pages = pages.Select(PageMapper.ToDto).ToList();
            ViewBag.MenuItems = await _menuService.GetAllMenuItemsAsync();
            return View("EditMenuItem", new MenuItem { Name = String.Empty });
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
        public async Task<IActionResult> UploadImage(IFormFile? file)
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
            return PartialView("_Partners", partners.ToList());
        }

        public async Task<IActionResult> MainSlider()
        {
            var slides = await _mainSliderService.GetAdminSlidesAsync();
            return PartialView("_MainSlider", slides);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMainSliderSlide(byte slot, IFormFile? imageFile, string? altText)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Оберіть зображення для завантаження.");
                var currentSlides = await _mainSliderService.GetAdminSlidesAsync();
                return PartialView("_MainSlider", currentSlides);
            }

            await _mainSliderService.UpdateSlideAsync(slot, imageFile, altText);
            var slides = await _mainSliderService.GetAdminSlidesAsync();
            return PartialView("_MainSlider", slides);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMainSliderSlide(byte slot)
        {
            try
            {
                await _mainSliderService.ClearSlideAsync(slot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete main slider slide {Slot}", slot);
                ModelState.AddModelError(string.Empty, "Не вдалося видалити слайд. Спробуйте ще раз.");
            }

            var slides = await _mainSliderService.GetAdminSlidesAsync();
            return PartialView("_MainSlider", slides);
        }

        [HttpPost]
        public async Task<IActionResult> UploadPartner(IFormFile? file, string name, string link)
        {
            if (file == null || file.Length == 0)
            {
                return RedirectToAction("Index", new { section = "Partners" });
            }

            var webPath = await _fileStorageService.SaveImage(file, "partners");

            await _partnerService.AddPartnerAsync(webPath, name, link);

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
            return Ok();
        }

        public async Task<IActionResult> Posts()
        {
            var posts = await _postService.GetAllAsync();
            _logger.LogInformation("Admin requested posts list, total {PostCount} posts", posts.Count());
            return PartialView("_PostsTable", posts);
        }

        public IActionResult CreatePost()
        {
            return PartialView("_CreatePost", new Post
            {
                Id = 0,
                Slug = string.Empty,
                Title = string.Empty,
                Content = string.Empty,
                Status = PostStatus.Draft,
                CreatedAt = DateTime.Now
            });
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post == null) return NotFound();

            return PartialView("_EditPost", post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePostDraft(Post post)
        {
            ClearPostSoftValidationState();

            try
            {
                NormalizePostForDraft(post);
                post.AuthorId = 1;
                if (post.CreatedAt == default)
                    post.CreatedAt = DateTime.Now;
                post.UpdatedAt = DateTime.Now;

                if (post.Id == 0)
                {
                    var id = await _postService.CreateAsync(post);
                    return Json(new { id, savedAt = DateTime.Now, status = "Draft" });
                }

                var existing = await _postService.GetByIdAsync(post.Id);
                if (existing == null) return NotFound();

                if (string.IsNullOrEmpty(post.Thumbnail))
                    post.Thumbnail = existing.Thumbnail;

                await _postService.UpdateAsync(post);
                return Json(new { id = post.Id, savedAt = DateTime.Now, status = "Draft" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка збереження чернетки");
                return StatusCode(500, "Помилка сервера");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishPost(Post post, IFormFile? thumbnailFile)
        {
            ClearPostSoftValidationState();

            if (!ValidatePostForPublish(post, out var error))
            {
                ModelState.AddModelError(string.Empty, error!);
                return post.Id == 0
                    ? PartialView("_CreatePost", post)
                    : PartialView("_EditPost", post);
            }

            try
            {
                post.Status = PostStatus.Published;
                post.AuthorId = 1;
                if (post.CreatedAt == default)
                    post.CreatedAt = DateTime.Now;
                post.UpdatedAt = DateTime.Now;
                post.Excerpt ??= string.Empty;

                if (thumbnailFile != null)
                    post.Thumbnail = await _fileStorageService.SaveImage(thumbnailFile, "posts");
                else if (post.Id != 0)
                {
                    var existing = await _postService.GetByIdAsync(post.Id);
                    if (existing != null && string.IsNullOrEmpty(post.Thumbnail))
                        post.Thumbnail = existing.Thumbnail;
                }

                if (post.Id == 0)
                    post.Id = await _postService.CreateAsync(post);
                else
                    await _postService.UpdateAsync(post);

                return await Posts();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while editing a post");
                return StatusCode(500, "Помилка сервера");
            }
        }

        private void ClearPostSoftValidationState()
        {
            ModelState.Remove("Author");
            ModelState.Remove("Thumbnail");
            ModelState.Remove(nameof(Post.Title));
            ModelState.Remove(nameof(Post.Slug));
            ModelState.Remove(nameof(Post.Content));
            ModelState.Remove(nameof(Post.Excerpt));
            ModelState.Remove(nameof(Post.Status));
        }

        private static void NormalizePostForDraft(Post post)
        {
            if (string.IsNullOrWhiteSpace(post.Title))
                post.Title = "Без назви";
            if (string.IsNullOrWhiteSpace(post.Slug))
                post.Slug = "draft-" + Guid.NewGuid().ToString("N")[..8];
            post.Excerpt ??= string.Empty;
            post.Status = PostStatus.Draft;
        }

        private static bool ValidatePostForPublish(Post post, out string? error)
        {
            if (string.IsNullOrWhiteSpace(post.Title))
            {
                error = "Заголовок є обов'язковим";
                return false;
            }
            if (string.IsNullOrWhiteSpace(post.Slug))
            {
                error = "Slug є обов'язковим";
                return false;
            }
            if (string.IsNullOrWhiteSpace(post.Content))
            {
                error = "Контент не може бути порожнім";
                return false;
            }
            error = null;
            return true;
        }


        private static List<string> ParseDriveLinks(string? linksText)
        {
            if (string.IsNullOrWhiteSpace(linksText))
                return new List<string>();

            return linksText
                .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(link => link.Trim())
                .Where(link => Uri.TryCreate(link, UriKind.Absolute, out var uri)
                               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                .Distinct()
                .Take(15)
                .ToList();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _postService.DeleteAsync(id);
            _logger.LogInformation("Admin deleted post with ID {PostId}", id);
            return await Posts();
        }
    }
}
