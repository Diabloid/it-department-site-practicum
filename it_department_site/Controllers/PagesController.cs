using Microsoft.AspNetCore.Mvc;
using it_department_site.Database;
using System.Data;
using dbhelper;

namespace it_department_site.Controllers
{
    public class PagesController : Controller
    {
        private readonly Dbhelper _dbHelper;

        public PagesController(Dbhelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
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
                    Convert.ToDateTime(row["created_at"]),
                    Convert.ToDateTime(row["updated_at"])
                ));
            }

            return View(pages); // Передаємо список сторінок у представлення Index.cshtml
        }

        // Метод для відображення окремої сторінки
        public IActionResult Details(string slug)
        {
            string query = $"SELECT * FROM pages WHERE slug = '{slug}'";
            var dt = _dbHelper.ExecuteQuery(query);

            if (dt.Rows.Count == 0)
            {
                return NotFound(); // Якщо сторінку не знайдено, повертаємо 404
            }

            DataRow row = dt.Rows[0];

            PageStatus status = Enum.TryParse(row["status_"].ToString(), out PageStatus parsedStatus)
                ? parsedStatus
                : PageStatus.Draft;

            Page page = new Page(
                Convert.ToInt32(row["id"]),
                row["title"].ToString(),
                row["slug"].ToString(),
                status,
                row["content"].ToString(),
                Convert.ToDateTime(row["created_at"]),
                Convert.ToDateTime(row["updated_at"])
            );

            return View(page); // Передаємо об'єкт сторінки у View
        }
    }
}
