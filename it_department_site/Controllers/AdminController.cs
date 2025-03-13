using Microsoft.AspNetCore.Mvc;
using it_department_mvc.Models;
using it_department_site.Database;
using System.Collections.Generic;
using System.Data;

namespace it_department_mvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly Dbhelper _dbHelper;

        public AdminController(Dbhelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Метод для відображення меню та контенту
        public IActionResult Index()
        {
            return View();
        }

        // Метод для відображення всіх сторінок
        public IActionResult Pages()
        {
            string query = "SELECT * FROM pages";
            var dt = _dbHelper.ExecuteQuery(query);

            List<Page> pages = new List<Page>();
            foreach (DataRow row in dt.Rows)
            {
                PageStatus status = Enum.TryParse(row["status_"].ToString(), out PageStatus parsedStatus)
                    ? parsedStatus
                    : PageStatus.Draft;
                pages.Add(new Page(
                    Convert.ToInt32(row["id"]),
                    row["title"].ToString(),
                    row["slug"].ToString(),
                    status,
                    row["content"].ToString(),
                    Convert.ToDateTime(row["createdAt"]),
                    Convert.ToDateTime(row["updatedAt"])
                ));
            }

            return View(pages);  // Відправка списку сторінок на вигляд
        }
    }
}
