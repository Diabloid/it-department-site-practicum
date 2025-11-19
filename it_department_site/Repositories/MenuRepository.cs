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
        public void LinkPageToMenu(int pageId, int? parentId, string title)
        {
            var existingMenu = _dbHelper.ExecuteQuery($"SELECT id FROM menu_items WHERE page_id = {pageId}");

            if (existingMenu.Rows.Count > 0)
            {
                int menuItemId = Convert.ToInt32(existingMenu.Rows[0]["id"]);
                _dbHelper.ExecuteNonQuery($@"
            UPDATE menu_items 
            SET name = '{title.Replace("'", "''")}', 
                parent_id = {(parentId?.ToString() ?? "NULL")}
            WHERE id = {menuItemId}
        ");
            }
            else
            {
                _dbHelper.ExecuteNonQuery($@"
            INSERT INTO menu_items (name, parent_id, page_id, sort_order)
            VALUES ('{title.Replace("'", "''")}', {(parentId?.ToString() ?? "NULL")}, {pageId}, 0)
        ");
            }
        }
        public MenuItem GetById(int id)
        {
            var row = _dbHelper.ExecuteQuery($"SELECT * FROM menu_items WHERE id = {id}").Rows[0];
            return new MenuItem
            {
                Id = Convert.ToInt32(row["id"]),
                Name = row["name"].ToString(),
                ParentId = row["parent_id"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["parent_id"]),
                PageId = row["page_id"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["page_id"]),
                SortOrder = Convert.ToInt32(row["sort_order"])
            };
        }

        public void Create(MenuItem item)
        {
            _dbHelper.ExecuteNonQuery($@"
        INSERT INTO menu_items (name, parent_id, page_id, sort_order)
        VALUES (
            '{item.Name.Replace("'", "''")}', 
            {(item.ParentId?.ToString() ?? "NULL")}, 
            {(item.PageId?.ToString() ?? "NULL")}, 
            {item.SortOrder}
        )
    ");
        }

        public void Update(MenuItem item)
        {
            _dbHelper.ExecuteNonQuery($@"
        UPDATE menu_items SET 
            name = '{item.Name.Replace("'", "''")}',
            parent_id = {(item.ParentId?.ToString() ?? "NULL")},
            page_id = {(item.PageId?.ToString() ?? "NULL")},
            sort_order = {item.SortOrder}
        WHERE id = {item.Id}
    ");
        }

        public void Delete(int id)
        {
            _dbHelper.ExecuteNonQuery($"DELETE FROM menu_items WHERE id = {id}");
        }

    }
}
