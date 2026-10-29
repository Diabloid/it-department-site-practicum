namespace it_department_site.Controllers
{
    public class PagesController(IPageService pageService, ICurrentUserService currentUser) : Controller
    {
        public async Task<IActionResult> Details(string slug)
        {
            bool isAdmin = currentUser.IsAuthenticated && currentUser.IsAdmin;

            var page = await pageService.GetPageBySlugAsync(slug, isAdmin);
            if (page == null)
            {
                return NotFound();
            }

            // НОВА ЛОГІКА ПЕРЕГЛЯДІВ (COOKIES)
            string viewCookieName = $"viewed_page_{page.Id}";

            if (!Request.Cookies.ContainsKey(viewCookieName))
            {
                await pageService.IncrementViewsAsync(page.Id);

                page.ViewsCount++;

                CookieOptions options = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(24), // Діє 24 години
                    HttpOnly = true, // Захист від XSS
                    IsEssential = true
                };

                Response.Cookies.Append(viewCookieName, "true", options);
            }

            ViewBag.Views = page.ViewsCount;
            var dto = PageMapper.ToDto(page);
            return View(dto);
        }

        [HttpPost]
        [Route("ckeditor/upload")]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }

                var fileUrl = Url.Content($"~/uploads/{fileName}");

                return Json(new { uploaded = true, url = fileUrl });
            }

            return Json(new { uploaded = false, error = new { message = "Error" } });
        }
    }
}
