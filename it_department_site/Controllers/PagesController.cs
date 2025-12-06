using Microsoft.AspNetCore.Mvc;
using it_department_site.Services;
using it_department_site.DTOs;
using it_department_site.Mappers;

namespace it_department_site.Controllers
{
    public class PagesController : Controller
    {
        private readonly IPageService _pageService;
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;
        private readonly ICurrentUserService _currentUser; // ✅

        public PagesController(IPageService pageService, ICommentService commentService, ILikeService likeService, ICurrentUserService currentUser)
        {
            _pageService = pageService;
            _commentService = commentService;
            _likeService = likeService;
            _currentUser = currentUser;
        }

        // Показ списку всіх сторінок
        public IActionResult Index()
        {
            var pages = _pageService.GetAllPages();
            var dtoList = pages.Select(PageMapper.ToDto).ToList();

            return View(dtoList);
        }

        // Деталі сторінки за slug
        public IActionResult Details(string slug)
        {
            // ✅ Стало:
            bool isAdmin = _currentUser.IsAdmin;

            var page = _pageService.GetPageBySlug(slug, isAdmin);
            if (page == null)
                return View("~/Views/Shared/NotFound.cshtml");

            // ✅ Логіка переглядів стала простою:
            if (_currentUser.IsAuthenticated)
            {
                int userId = _currentUser.UserId.Value;
                if (!_pageService.HasUserViewedPage(userId, page.Id))
                {
                    _pageService.MarkPageAsViewed(userId, page.Id);
                }
            }

            ViewBag.Comments = _commentService.GetComments(page.Id);
            ViewBag.Likes = _likeService.GetLikeCount(page.Id);
            ViewBag.Views = _pageService.GetViews(page.Id);

            var dto = PageMapper.ToDto(page);
            return View(dto);
        }

        [HttpGet]
        public JsonResult SearchSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new List<object>());

            var results = _pageService.Search(term)
                .Select(p => new { title = p.Title, slug = p.Slug })
                .Distinct()
                .Take(10)
                .ToList();

            return Json(results);
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

                return Json(new
                {
                    uploaded = true,
                    url = fileUrl
                });
            }

            return Json(new
            {
                uploaded = false,
                error = new { message = "Не вдалося завантажити файл" }
            });
        }
    }
}