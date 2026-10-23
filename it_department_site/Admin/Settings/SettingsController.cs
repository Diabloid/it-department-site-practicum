using Microsoft.AspNetCore.Mvc;

namespace it_department_site.Admin.Settings
{
    public class SettingsController(
        ISiteSettingsService settingsService,
        IPageService pageService,
        IFileStorageService fileStorageService,
        IOrganizationInfoService organizationInfoService) : AdminControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var info = await organizationInfoService.GetAsync();

            ViewBag.SiteSettings = await settingsService.GetAsync();
            ViewBag.Pages = await pageService.GetAllPagesAsync();

            return View("Settings", info);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OrganizationInfo model)
        {
            ModelState.Clear();

            model.PhoneNumbers = model.PhoneNumbers.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
            model.SocialLinks = model.SocialLinks.Where(s => !string.IsNullOrWhiteSpace(s.Url)).ToList();

            await organizationInfoService.UpdateAsync(model);

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

            var current = await settingsService.GetAsync();
            current.SiteTitle = model.SiteTitle;
            current.HomePageId = model.HomePageId;

            if (logoFile != null)
            {
                current.SiteLogoUrl = await fileStorageService.SaveImage(logoFile, string.Empty);
            }

            await settingsService.SaveAsync(current);
            TempData["Success"] = "Зміни збережено!";
            return RedirectToAction(nameof(Index));
        }
    }
}
