using dbhelper;
using it_department_site.Database;
using System.Data;

namespace it_department_site.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly Dbhelper _dbHelper;

        public MenuRepository(Dbhelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<MenuItem> GetAll()
        {
            var dt = _dbHelper.ExecuteQuery("SELECT * FROM menu_items ORDER BY sort_order");
            var menuItems = new List<MenuItem>();

            foreach (DataRow row in dt.Rows)
            {
                menuItems.Add(new MenuItem
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = row["name"].ToString(),
                    ParentId = row["parent_id"] == DBNull.Value ? null : Convert.ToInt32(row["parent_id"]),
                    PageId = row["page_id"] == DBNull.Value ? null : Convert.ToInt32(row["page_id"]),
                    SortOrder = Convert.ToInt32(row["sort_order"])
                });
            }

            return menuItems;
        }
    }
}
