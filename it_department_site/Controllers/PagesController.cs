using Microsoft.AspNetCore.Mvc;
using it_department_site.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace it_department_site.Controllers
{
    public class PagesController : Controller
    {
        private readonly IPageService _pageService;

        public PagesController(IPageService pageService)
        {
            _pageService = pageService;
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
            return View(pages);
        }

        // Деталі сторінки за slug
        public IActionResult Details(string slug)
        {
            bool isAdmin = IsAdminFromJwt();

            var page = _pageService.GetPageBySlug(slug, isAdmin);

            if (page == null)
                return View("~/Views/Shared/NotFound.cshtml");

            return View(page);
        }
    }
}
