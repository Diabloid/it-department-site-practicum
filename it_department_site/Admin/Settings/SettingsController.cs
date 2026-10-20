using Microsoft.AspNetCore.Mvc;

namespace it_department_site.Admin.Settings
{
    public class SettingsController : AdminControllerBase
    {
        private readonly ISiteSettingsService _settingsService;
        private readonly IPageService _pageService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IOrganizationInfoService _organizationInfoService;

        public SettingsController(
            ISiteSettingsService settingsService,
            IPageService pageService,
            IFileStorageService fileStorageService,
            IOrganizationInfoService organizationInfoService)
        {
            _settingsService = settingsService;
            _pageService = pageService;
            _fileStorageService = fileStorageService;
            _organizationInfoService = organizationInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var info = await _organizationInfoService.GetAsync();

            ViewBag.SiteSettings = await _settingsService.GetAsync();
            ViewBag.Pages = await _pageService.GetAllPagesAsync();

            return View("Settings", info);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OrganizationInfo model)
        {
            ModelState.Clear();

            model.PhoneNumbers = model.PhoneNumbers.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
            model.SocialLinks = model.SocialLinks.Where(s => !string.IsNullOrWhiteSpace(s.Url)).ToList();

            await _organizationInfoService.UpdateAsync(model);

            TempData["SuccessMessage"] = "Налаштування організації успішно збережено!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LayoutSettings(SiteSettings model, IFormFile? logoFile)
        {
            if (string.IsNullOrWhiteSpace(model.SiteTitle))
            {
                TempData["Error"] = "Назва сайту не може бути порожньою!";
                return RedirectToAction(nameof(Index));
            }

            var current = await _settingsService.GetAsync();
            current.SiteTitle = model.SiteTitle;
            current.HomePageId = model.HomePageId;

            if (logoFile != null)
            {
                current.SiteLogoUrl = await _fileStorageService.SaveImage(logoFile, string.Empty);
            }

            await _settingsService.SaveAsync(current);
            TempData["Success"] = "Зміни збережено!";
            return RedirectToAction(nameof(Index));
        }
    }
}
