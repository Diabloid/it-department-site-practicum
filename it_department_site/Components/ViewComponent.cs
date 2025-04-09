using Microsoft.AspNetCore.Mvc;
using it_department_site.Database;
using dbhelper;
using System.Data;

namespace it_department_site.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly Dbhelper _dbHelper;

        public MenuViewComponent(Dbhelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IViewComponentResult Invoke()
        {
            var dt = _dbHelper.ExecuteQuery("SELECT * FROM menu_items ORDER BY sort_order");
            var menuItems = new List<MenuItem>();

            foreach (DataRow row in dt.Rows)
            {
                var menuItem = new MenuItem
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = row["name"].ToString(),
                    ParentId = row["parent_id"] != DBNull.Value ? Convert.ToInt32(row["parent_id"]) : null,
                    PageId = row["page_id"] != DBNull.Value ? Convert.ToInt32(row["page_id"]) : null,
                    SortOrder = Convert.ToInt32(row["sort_order"])
                };

                if (menuItem.PageId.HasValue)
                {
                    var slugDt = _dbHelper.ExecuteQuery($"SELECT slug FROM pages WHERE id = {menuItem.PageId.Value}");
                    if (slugDt.Rows.Count > 0)
                        menuItem.PageSlug = slugDt.Rows[0]["slug"].ToString();
                }

                menuItems.Add(menuItem);
            }

            var lookup = menuItems.ToDictionary(m => m.Id);
            var rootItems = new List<MenuItem>();

            foreach (var item in menuItems)
            {
                if (item.ParentId.HasValue && lookup.ContainsKey(item.ParentId.Value))
                {
                    lookup[item.ParentId.Value].Children.Add(item);
                }
                else
                {
                    rootItems.Add(item);
                }
            }

            return View("_MenuPartial", rootItems);
        }
    }
}
