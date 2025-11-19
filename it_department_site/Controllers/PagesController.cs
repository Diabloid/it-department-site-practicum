using Microsoft.AspNetCore.Mvc;
using it_department_site.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using it_department_site.DTOs;
using it_department_site.Mappers;

namespace it_department_site.Controllers
{
    public class PagesController : Controller
    {
        private readonly IPageService _pageService;
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;

        public PagesController(IPageService pageService, ICommentService commentService, ILikeService likeService)
        {
            _pageService = pageService;
            _commentService = commentService;
            _likeService = likeService;
        }
        private bool IsAdminFromJwt()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token)) return false;

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwt = handler.ReadJwtToken(token);
                var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                return roleClaim?.Value == "Admin";
            }
            catch
            {
                return false;
            }
        }

        // Показ списку всіх сторінок
        public IActionResult Index()
        {
            var pages = _pageService.GetAllPages();
            var dtoList = pages.Select(PageMapper.ToDto).ToList();

            return View(dtoList); // ← List<PageDto>
        }

        // Деталі сторінки за slug
        public IActionResult Details(string slug)
        {
            bool isAdmin = IsAdminFromJwt();
            var page = _pageService.GetPageBySlug(slug, isAdmin);
            if (page == null)
                return View("~/Views/Shared/NotFound.cshtml");

            var jwt = Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(jwt))
            {
                var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
                var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(userIdClaim, out int userId))
                {
                    if (!_pageService.HasUserViewedPage(userId, page.Id))
                    {
                        _pageService.MarkPageAsViewed(userId, page.Id);
                    }
                }
            }

            ViewBag.Comments = _commentService.GetComments(page.Id);
            ViewBag.Likes = _likeService.GetLikeCount(page.Id);
            ViewBag.Views = _pageService.GetViews(page.Id);

            var dto = PageMapper.ToDto(page); // page вже з оновленим Views
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
                Directory.CreateDirectory(uploadsFolder); // на випадок, якщо нема

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
