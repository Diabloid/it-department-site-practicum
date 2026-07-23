namespace it_department_site.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly IDbConnection _db;

        public MenuRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<List<MenuItem>> GetAllAsync()
        {
            var sql = $@"
                SELECT 
                    m.id, 
                    m.name, 
                    m.parent_id as ParentId, 
                    m.page_id as PageId,
                    m.custom_url as CustomUrl,
                    m.sort_order as SortOrder,
                    p.slug as PageSlug
                FROM menu_items m
                LEFT JOIN pages p ON m.page_id = p.id
                ORDER BY m.sort_order";

            var result = await _db.QueryAsync<MenuItem>(sql);
            return result.ToList();
        }

        public async Task LinkPageToMenuAsync(int pageId, int? parentId, string title)
        {
            var existingId = await _db.ExecuteScalarAsync<int?>("SELECT id FROM menu_items WHERE page_id = @pageId", new { pageId });

            if (existingId.HasValue)
            {
                var updateSql = "UPDATE menu_items SET name = @title, parent_id = @parentId WHERE id = @id";
                await _db.ExecuteAsync(updateSql, new { title, parentId, id = existingId.Value });
            }
            else
            {
                var insertSql = "INSERT INTO menu_items (name, parent_id, page_id, sort_order) VALUES (@title, @parentId, @pageId, 0)";
                await _db.ExecuteAsync(insertSql, new { title, parentId, pageId });
            }
        }

        public async Task<MenuItem> GetByIdAsync(int id)
        {
            return await _db.QueryFirstOrDefaultAsync<MenuItem>(
                $"SELECT id, name, parent_id as ParentId, page_id as PageId, custom_url as CustomUrl, sort_order as SortOrder FROM menu_items WHERE id = @id",
                new { id });
        }

        public async Task CreateAsync(MenuItem item)
        {
            var sql = @"
                INSERT INTO menu_items (name, parent_id, page_id, custom_url, sort_order) 
                VALUES (@Name, @ParentId, @PageId, @CustomUrl, @SortOrder)";
            await _db.ExecuteAsync(sql, item);
        }

        public async Task UpdateAsync(MenuItem item)
        {
                var sql = @"
                    UPDATE menu_items SET 
                        name = @Name, 
                        parent_id = @ParentId, 
                        page_id = @PageId,
                        custom_url = @CustomUrl,
                        sort_order = @SortOrder 
                    WHERE id = @Id";
                await _db.ExecuteAsync(sql, item);
        }

        public async Task DeleteAsync(int id)
        {
            await _db.ExecuteAsync("DELETE FROM menu_items WHERE id = @id", new { id });
        }
    }
}
