using Microsoft.AspNetCore.Mvc;
using it_department_site.Models;
using it_department_site.Database;
using System.Collections.Generic;
using System.Data;
using dbhelper;

namespace it_department_site.Controllers
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
                    Convert.ToDateTime(row["created_at"]),
                    Convert.ToDateTime(row["updated_at"])
                ));
            }

            return PartialView("_PagesTable",pages);  // Відправка списку сторінок на вигляд
        }
        public IActionResult Users()
        {
            string query = "SELECT * FROM users";
            var dt = _dbHelper.ExecuteQuery(query);
            return PartialView("_UsersTable", dt);
        }
        // Сторінка редактора
        public IActionResult PageEditor(int? id)
        {
            PageEditorViewModel model;

            if (id == null)
            {
                model = new PageEditorViewModel();
            }
            else
            {
                var dt = _dbHelper.ExecuteQuery($"SELECT * FROM pages WHERE id = {id}");

                if (dt.Rows.Count == 0)
                    return NotFound();

                var row = dt.Rows[0];

                model = new PageEditorViewModel
                {
                    Id = Convert.ToInt32(row["id"]),
                    Title = row["title"].ToString(),
                    Slug = row["slug"].ToString(),
                    Content = row["content"].ToString(),
                    LayoutTemplate = "" // Не зберігається в БД, задається у редакторі
                };
            }

            // ✅ Завантажуємо меню, де parent_id IS NULL
            var menuItemsDt = _dbHelper.ExecuteQuery("SELECT * FROM menu_items ORDER BY sort_order");
            var menuItems = new List<MenuItem>();
            foreach (DataRow menuRow in menuItemsDt.Rows)
            {
                menuItems.Add(new MenuItem
                {
                    Id = Convert.ToInt32(menuRow["id"]),
                    Name = menuRow["name"].ToString(),
                });
            }
            ViewBag.MenuItems = menuItems;

            return View(model);
        }

        [HttpPost]
        public IActionResult SavePage(PageEditorViewModel model)
        {
            string query;

            if (model.Id == null)
            {
                // Додаємо нову сторінку
                query = $"INSERT INTO pages (title, slug, status_, content, created_at, updated_at) VALUES (" +
                        $"'{model.Title}', '{model.Slug}', 'Published', '{model.Content}', NOW(), NOW())";
            }
            else
            {
                // Оновлюємо існуючу сторінку
                query = $"UPDATE pages SET title = '{model.Title}', slug = '{model.Slug}', " +
                        $"content = '{model.Content}', updated_at = NOW() WHERE id = {model.Id}";
            }

            _dbHelper.ExecuteNonQuery(query);

            // Якщо вибрано пункт меню, додаємо його
            if (!string.IsNullOrEmpty(Request.Form["ParentMenuItemId"]))
            {
                int parentId = Convert.ToInt32(Request.Form["ParentMenuItemId"]);
                string insertMenuQuery = $@"
            INSERT INTO menu_items (name, parent_id, page_id, sort_order)
            VALUES ('{model.Title}', {parentId}, {model.Id}, 99)";
                _dbHelper.ExecuteNonQuery(insertMenuQuery);
            }

            return RedirectToAction("Index");
        }
    }
}
