using System.Diagnostics;
using it_department_mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace it_department_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult EducationalPrograms()
        {
            return View();
        }
        public IActionResult ELearning()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }
        public IActionResult Info()
        {
            return View();
        }
        public IActionResult Practices()
        {
            return View();
        }
        public IActionResult SocialSurveys()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
