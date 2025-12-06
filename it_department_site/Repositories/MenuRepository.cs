using Dapper;
using dbhelper;
using it_department_site.Database;
using it_department_site.Repositories;
using System.Collections.Generic;
using System.Linq;

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
            using var db = _dbHelper.GetConnection();
            var sql = @"
        SELECT 
            m.id, 
            m.name, 
            m.parent_id as ParentId, 
            m.page_id as PageId, 
            m.sort_order as SortOrder,
            p.slug as PageSlug    -- <--- Ось це поле нам потрібне для посилань!
        FROM menu_items m
        LEFT JOIN pages p ON m.page_id = p.id
        ORDER BY m.sort_order";

            return db.Query<MenuItem>(sql).ToList();
        }

        public void LinkPageToMenu(int pageId, int? parentId, string title)
        {
            using var db = _dbHelper.GetConnection();

            var existingId = db.ExecuteScalar<int?>("SELECT id FROM menu_items WHERE page_id = @pageId", new { pageId });

            if (existingId.HasValue)
            {
                var updateSql = "UPDATE menu_items SET name = @title, parent_id = @parentId WHERE id = @id";
                db.Execute(updateSql, new { title, parentId, id = existingId.Value });
            }
            else
            {
                var insertSql = "INSERT INTO menu_items (name, parent_id, page_id, sort_order) VALUES (@title, @parentId, @pageId, 0)";
                db.Execute(insertSql, new { title, parentId, pageId });
            }
        }

        public MenuItem GetById(int id)
        {
            using var db = _dbHelper.GetConnection();
            return db.QueryFirstOrDefault<MenuItem>(
                "SELECT id, name, parent_id as ParentId, page_id as PageId, sort_order as SortOrder FROM menu_items WHERE id = @id",
                new { id });
        }

        public void Create(MenuItem item)
        {
            using var db = _dbHelper.GetConnection();
            var sql = @"
                INSERT INTO menu_items (name, parent_id, page_id, sort_order) 
                VALUES (@Name, @ParentId, @PageId, @SortOrder)";
            db.Execute(sql, item);
        }

        public void Update(MenuItem item)
        {
            using var db = _dbHelper.GetConnection();
            var sql = @"
                UPDATE menu_items SET 
                    name = @Name, 
                    parent_id = @ParentId, 
                    page_id = @PageId, 
                    sort_order = @SortOrder 
                WHERE id = @Id";
            db.Execute(sql, item);
        }

        public void Delete(int id)
        {
            using var db = _dbHelper.GetConnection();
            db.Execute("DELETE FROM menu_items WHERE id = @id", new { id });
        }
    }
}