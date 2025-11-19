using it_department_site.Database;
using dbhelper;
using System.Data;
using it_department_site.Repositories;
using System.Text.Json;

public class PageRepository : IPageRepository
{
    private readonly Dbhelper _dbHelper;

    public PageRepository(Dbhelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public Page? GetBySlug(string slug)
    {
        var query = "SELECT * FROM pages WHERE slug = @slug";
        var parameters = new Dictionary<string, object>
        {
            { "@slug", slug }
        };

        var dt = _dbHelper.ExecuteQuery(query, parameters);
        return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
    }

    public Page? GetById(int id)
    {
        var query = "SELECT * FROM pages WHERE id = @id";
        var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };

        var dt = _dbHelper.ExecuteQuery(query, parameters);
        return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
    }

    public List<Page> GetAll()
    {
        var dt = _dbHelper.ExecuteQuery("SELECT * FROM pages");
        List<Page> pages = new();
        foreach (DataRow row in dt.Rows)
        {
            pages.Add(MapRow(row));
        }
        return pages;
    }

    public int Create(Page page)
    {
        var query = @"INSERT INTO pages 
        (title, slug, status_, content, layout_type, created_at, updated_at, allow_comments)
        VALUES (@title, @slug, @status, @content, @layout_type, NOW(), NOW(), @allow_comments);
        SELECT LAST_INSERT_ID();";

        var parameters = new Dictionary<string, object>
    {
        { "@title", page.Title },
        { "@slug", page.Slug },
        { "@status", page.Status.ToString() },
        { "@content", page.ContentJson },
        { "@layout_type", page.LayoutType },
        { "@allow_comments", page.AllowComments }
    };

        var result = _dbHelper.ExecuteQuery(query, parameters);
        return Convert.ToInt32(result.Rows[0][0]);
    }

    public void Update(Page page)
    {
        var query = @"UPDATE pages SET 
                title = @title,
                slug = @slug,
                status_ = @status,
                content = @content,
                layout_type = @layout_type,
                allow_comments = @allow_comments,
                updated_at = NOW()
              WHERE id = @id";

        var parameters = new Dictionary<string, object>
{
    { "@id", page.Id },
    { "@title", page.Title },
    { "@slug", page.Slug },
    { "@status", page.Status.ToString() },
    { "@content", page.ContentJson },
    { "@layout_type", page.LayoutType },
    { "@allow_comments", page.AllowComments }
};

        _dbHelper.ExecuteNonQuery(query, parameters);
    }

    public void Delete(int id)
    {
        var query = "DELETE FROM pages WHERE id = @id";
        var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };
        _dbHelper.ExecuteNonQuery(query, parameters);
    }

    private Page MapRow(DataRow row)
    {
        PageStatus status = Enum.TryParse(row["status_"].ToString(), out PageStatus s) ? s : PageStatus.Draft;
        string layoutType = row["layout_type"].ToString();
        string contentJson = row["content"].ToString();
        bool allowComments = row.Table.Columns.Contains("allow_comments") && row["allow_comments"] != DBNull.Value && Convert.ToBoolean(row["allow_comments"]);

        return new Page(
            Convert.ToInt32(row["id"]),
            row["title"].ToString(),
            row["slug"].ToString(),
            status,
            contentJson,
            layoutType,
            Convert.ToDateTime(row["created_at"]),
            Convert.ToDateTime(row["updated_at"]),
            allowComments // ← додати
        );
    }

    public List<Page> GetPaged(int pageNumber, int pageSize)
    {
        int offset = (pageNumber - 1) * pageSize;
        string query = $"SELECT * FROM pages ORDER BY id DESC LIMIT {pageSize} OFFSET {offset}";

        var dt = _dbHelper.ExecuteQuery(query);
        List<Page> pages = new();
        foreach (DataRow row in dt.Rows)
        {
            pages.Add(MapRow(row));
        }
        return pages;
    }

    public int GetTotal(int pageSize)
    {
        var dt = _dbHelper.ExecuteQuery("SELECT COUNT(*) AS total FROM pages");
        int total = Convert.ToInt32(dt.Rows[0]["total"]);
        return (int)Math.Ceiling((double)total / pageSize);
    }

    public List<Page> Search(string query)
    {
        string sql = @"SELECT * FROM pages 
                       WHERE LOWER(title) LIKE CONCAT('%', LOWER(@q), '%') 
                          OR LOWER(slug) LIKE CONCAT('%', LOWER(@q), '%')";

        var parameters = new Dictionary<string, object>
        {
            { "@q", query }
        };

        var dt = _dbHelper.ExecuteQuery(sql, parameters);
        List<Page> results = new();
        foreach (DataRow row in dt.Rows)
        {
            results.Add(MapRow(row));
        }
        return results;
    }
    public bool HasPageView(int userId, int pageId)
    {
        var sql = "SELECT 1 FROM PageViews WHERE UserId = @userId AND PageId = @pageId LIMIT 1";
        var parameters = new Dictionary<string, object>
    {
        { "@userId", userId },
        { "@pageId", pageId }
    };

        var dt = _dbHelper.ExecuteQuery(sql, parameters);
        return dt.Rows.Count > 0;
    }

    public void AddPageView(int userId, int pageId)
    {
        var sql = "INSERT INTO PageViews (UserId, PageId) VALUES (@userId, @pageId)";
        var parameters = new Dictionary<string, object>
    {
        { "@userId", userId },
        { "@pageId", pageId }
    };

        _dbHelper.ExecuteNonQuery(sql, parameters);
    }

    public void IncrementViews(int pageId)
    {
        var sql = "UPDATE Pages SET Views = Views + 1 WHERE Id = @pageId";
        var parameters = new Dictionary<string, object>
    {
        { "@pageId", pageId }
    };

        _dbHelper.ExecuteNonQuery(sql, parameters);
    }
    public int GetViews(int pageId)
    {
        var sql = "SELECT Views FROM pages WHERE Id = @id";
        var parameters = new Dictionary<string, object>
    {
        { "@id", pageId }
    };

        var dt = _dbHelper.ExecuteQuery(sql, parameters);
        if (dt.Rows.Count > 0 && int.TryParse(dt.Rows[0]["Views"].ToString(), out int views))
        {
            return views;
        }

        return 0;
    }
}
