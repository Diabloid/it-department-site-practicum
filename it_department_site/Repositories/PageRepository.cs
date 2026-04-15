public class PageRepository : IPageRepository
{
    private readonly IDbConnection _db;

    public PageRepository(IDbConnection db)
    {
        _db = db;
    }

    private const string BaseSelectSql = @"
        SELECT 
            id, 
            title, 
            slug, 
            status_ as Status, 
            content as ContentJson, 
            layout_type as LayoutType, 
            created_at as CreatedAt, 
            updated_at as UpdatedAt, 
            allow_comments as AllowComments, 
            Views 
        FROM pages";

    public async Task<Page?> GetBySlugAsync(string slug)
    {
        return await _db.QueryFirstOrDefaultAsync<Page>(
            $"{BaseSelectSql} WHERE slug = @slug", new { slug });
    }

    public async Task<Page?> GetByIdAsync(int id)
    {
        return await _db.QueryFirstOrDefaultAsync<Page>(
            $"{BaseSelectSql} WHERE id = @id", new { id });
    }

    public async Task<List<Page>> GetAllAsync()
    {
        var result = await _db.QueryAsync<Page>(BaseSelectSql);
        return result.ToList();
    }

    public async Task<int> CreateAsync(Page page)
    {
        var sql = @"
            INSERT INTO pages 
            (title, slug, status_, content, layout_type, created_at, updated_at, allow_comments, Views)
            VALUES 
            (@Title, @Slug, @Status, @ContentJson, @LayoutType, NOW(), NOW(), @AllowComments, 0);
            SELECT LAST_INSERT_ID();";

        return await _db.QuerySingleAsync<int>(sql, page);
    }

    public async Task UpdateAsync(Page page)
    {
        var sql = @"
            UPDATE pages SET 
                title = @Title,
                slug = @Slug,
                status_ = @Status,
                content = @ContentJson,
                layout_type = @LayoutType,
                allow_comments = @AllowComments,
                updated_at = NOW()
            WHERE id = @Id";

        await _db.ExecuteAsync(sql, page);
    }

    public async Task DeleteAsync(int id)
    {
        await _db.ExecuteAsync("DELETE FROM pages WHERE id = @id", new { id });
    }

    public async Task<List<Page>> GetPagedAsync(int pageNumber, int pageSize)
    {
        int offset = (pageNumber - 1) * pageSize;
        var sql = $"{BaseSelectSql} ORDER BY id DESC LIMIT @pageSize OFFSET @offset";

        var result = await _db.QueryAsync<Page>(sql, new { pageSize, offset });
        return result.ToList();
    }

    public async Task<int> GetTotalAsync(int pageSize)
    {
        int total = await _db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM pages");
        return (int)Math.Ceiling((double)total / pageSize);
    }


    // --- Перегляди ---

    public async Task<bool> HasPageViewAsync(int userId, int pageId)
    {
        int? result = await _db.QueryFirstOrDefaultAsync<int?>(
            "SELECT 1 FROM page_views WHERE UserId = @userId AND PageId = @pageId LIMIT 1",
            new { userId, pageId });
        return result.HasValue;
    }

    public async Task AddPageViewAsync(int userId, int pageId)
    {
        await _db.ExecuteAsync(
            "INSERT INTO page_views (UserId, PageId) VALUES (@userId, @pageId)",
            new { userId, pageId });
    }

    public async Task IncrementViewsAsync(int pageId)
    {
        await _db.ExecuteAsync(
            "UPDATE pages SET Views = Views + 1 WHERE Id = @pageId",
            new { pageId });
    }

    public async Task<int> GetViewsAsync(int pageId)
    {
        return await _db.ExecuteScalarAsync<int>("SELECT Views FROM pages WHERE Id = @id", new { id = pageId });
    }
}