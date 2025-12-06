using Dapper; // ✅ Обов'язково додай цей using
using it_department_site.Database;
using dbhelper;
using it_department_site.Repositories;
using System.Data;

public class PageRepository : IPageRepository
{
    private readonly Dbhelper _dbHelper;

    public PageRepository(Dbhelper dbHelper)
    {
        _dbHelper = dbHelper;
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

    public Page? GetBySlug(string slug)
    {
        using var db = _dbHelper.GetConnection();
        return db.QueryFirstOrDefault<Page>(
            $"{BaseSelectSql} WHERE slug = @slug",
            new { slug });
    }

    public Page? GetById(int id)
    {
        using var db = _dbHelper.GetConnection();
        return db.QueryFirstOrDefault<Page>(
            $"{BaseSelectSql} WHERE id = @id",
            new { id });
    }

    public List<Page> GetAll()
    {
        using var db = _dbHelper.GetConnection();
        return db.Query<Page>(BaseSelectSql).ToList();
    }

    public int Create(Page page)
    {
        using var db = _dbHelper.GetConnection();
        var sql = @"
            INSERT INTO pages 
            (title, slug, status_, content, layout_type, created_at, updated_at, allow_comments, Views)
            VALUES 
            (@Title, @Slug, @Status, @ContentJson, @LayoutType, NOW(), NOW(), @AllowComments, 0);
            
            SELECT LAST_INSERT_ID();";

        return db.QuerySingle<int>(sql, page);
    }

    public void Update(Page page)
    {
        using var db = _dbHelper.GetConnection();
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

        db.Execute(sql, page);
    }

    public void Delete(int id)
    {
        using var db = _dbHelper.GetConnection();
        db.Execute("DELETE FROM pages WHERE id = @id", new { id });
    }

    public List<Page> GetPaged(int pageNumber, int pageSize)
    {
        using var db = _dbHelper.GetConnection();
        int offset = (pageNumber - 1) * pageSize;

        var sql = $"{BaseSelectSql} ORDER BY id DESC LIMIT @pageSize OFFSET @offset";

        return db.Query<Page>(sql, new { pageSize, offset }).ToList();
    }

    public int GetTotal(int pageSize)
    {
        using var db = _dbHelper.GetConnection();
        int total = db.ExecuteScalar<int>("SELECT COUNT(*) FROM pages");
        return (int)Math.Ceiling((double)total / pageSize);
    }

    public List<Page> Search(string query)
    {
        using var db = _dbHelper.GetConnection();
        var sql = $@"{BaseSelectSql} 
                     WHERE LOWER(title) LIKE CONCAT('%', LOWER(@q), '%') 
                        OR LOWER(slug) LIKE CONCAT('%', LOWER(@q), '%')";

        return db.Query<Page>(sql, new { q = query }).ToList();
    }

    // --- Методи переглядів (PageViews) ---

    public bool HasPageView(int userId, int pageId)
    {
        using var db = _dbHelper.GetConnection();
        int? result = db.QueryFirstOrDefault<int?>(
            "SELECT 1 FROM page_views WHERE UserId = @userId AND PageId = @pageId LIMIT 1",
            new { userId, pageId });

        return result.HasValue;
    }

    public void AddPageView(int userId, int pageId)
    {
        using var db = _dbHelper.GetConnection();
        db.Execute(
            "INSERT INTO page_views (UserId, PageId) VALUES (@userId, @pageId)",
            new { userId, pageId });
    }

    public void IncrementViews(int pageId)
    {
        using var db = _dbHelper.GetConnection();
        db.Execute(
            "UPDATE pages SET Views = Views + 1 WHERE Id = @pageId",
            new { pageId });
    }

    public int GetViews(int pageId)
    {
        using var db = _dbHelper.GetConnection();
        return db.ExecuteScalar<int>("SELECT Views FROM pages WHERE Id = @id", new { id = pageId });
    }
}