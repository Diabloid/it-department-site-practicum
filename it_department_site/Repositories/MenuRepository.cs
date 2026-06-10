namespace it_department_site.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly IDbConnection _db;
        private bool? _hasCustomUrlColumn;

        public MenuRepository(IDbConnection db)
        {
            _db = db;
        }

        private async Task<bool> HasCustomUrlColumnAsync()
        {
            if (_hasCustomUrlColumn.HasValue)
                return _hasCustomUrlColumn.Value;

            var count = await _db.ExecuteScalarAsync<int>(@"
                SELECT COUNT(*)
                FROM information_schema.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = 'menu_items'
                  AND COLUMN_NAME = 'custom_url'");

            _hasCustomUrlColumn = count > 0;
            return _hasCustomUrlColumn.Value;
        }

        public async Task<List<MenuItem>> GetAllAsync()
        {
            var hasCustomUrl = await HasCustomUrlColumnAsync();
            var customUrlSelect = hasCustomUrl ? "m.custom_url as CustomUrl," : "NULL as CustomUrl,";

            var sql = $@"
                SELECT 
                    m.id, 
                    m.name, 
                    m.parent_id as ParentId, 
                    m.page_id as PageId,
                    {customUrlSelect}
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
            var hasCustomUrl = await HasCustomUrlColumnAsync();
            var customUrlSelect = hasCustomUrl ? "custom_url as CustomUrl," : "NULL as CustomUrl,";

            return await _db.QueryFirstOrDefaultAsync<MenuItem>(
                $"SELECT id, name, parent_id as ParentId, page_id as PageId, {customUrlSelect} sort_order as SortOrder FROM menu_items WHERE id = @id",
                new { id });
        }

        public async Task CreateAsync(MenuItem item)
        {
            if (await HasCustomUrlColumnAsync())
            {
                var sql = @"
                    INSERT INTO menu_items (name, parent_id, page_id, custom_url, sort_order) 
                    VALUES (@Name, @ParentId, @PageId, @CustomUrl, @SortOrder)";
                await _db.ExecuteAsync(sql, item);
                return;
            }

            var sqlWithoutCustomUrl = @"
                INSERT INTO menu_items (name, parent_id, page_id, sort_order) 
                VALUES (@Name, @ParentId, @PageId, @SortOrder)";
            await _db.ExecuteAsync(sqlWithoutCustomUrl, item);
        }

        public async Task UpdateAsync(MenuItem item)
        {
            if (await HasCustomUrlColumnAsync())
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
                return;
            }

            var sqlWithoutCustomUrl = @"
                UPDATE menu_items SET 
                    name = @Name, 
                    parent_id = @ParentId, 
                    page_id = @PageId,
                    sort_order = @SortOrder 
                WHERE id = @Id";
            await _db.ExecuteAsync(sqlWithoutCustomUrl, item);
        }

        public async Task DeleteAsync(int id)
        {
            await _db.ExecuteAsync("DELETE FROM menu_items WHERE id = @id", new { id });
        }
    }
}
