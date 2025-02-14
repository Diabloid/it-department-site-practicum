using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace it_department_site.Controllers
{
    public class DbController : Controller
    {
        private readonly Dbhelper _dbHelper;

        public DbController(IConfiguration configuration)
        {
            _dbHelper = new Dbhelper(configuration);
        }

        public IActionResult Index()
        {
            string query = "SELECT * FROM users";
            DataTable users = _dbHelper.ExecuteQuery(query);
            return View("Index",users);
        }
    }
}
