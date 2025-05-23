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
        var dt = _dbHelper.ExecuteQuery($"SELECT * FROM pages WHERE slug = '{slug}'");
        return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
    }

    public Page? GetById(int id)
    {
        var dt = _dbHelper.ExecuteQuery($"SELECT * FROM pages WHERE id = {id}");
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

    public void Create(Page page)
    {

        string contentJson = System.Text.Json.JsonSerializer.Serialize(
    page.Content,
    new JsonSerializerOptions
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
        string q = $"INSERT INTO pages (title, slug, status_, content, layout_type, created_at, updated_at) " +
$"VALUES ('{page.Title}', '{page.Slug}', '{page.Status}', '{contentJson}', '{page.LayoutType}', NOW(), NOW())";
        _dbHelper.ExecuteNonQuery(q);
    }

    public void Update(Page page)
    {
        string contentJson = System.Text.Json.JsonSerializer.Serialize(
    page.Content,
    new JsonSerializerOptions
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });

        string q = $"UPDATE pages SET title = '{page.Title}', slug = '{page.Slug}', status_ = '{page.Status}', " +
$"content = '{contentJson}', layout_type = '{page.LayoutType}', updated_at = NOW() WHERE id = {page.Id}";
        _dbHelper.ExecuteNonQuery(q);
    }

    public void Delete(int id)
    {
        _dbHelper.ExecuteNonQuery($"DELETE FROM pages WHERE id = {id}");
    }

    private Page MapRow(DataRow row)
    {
        PageStatus status = Enum.TryParse(row["status_"].ToString(), out PageStatus s) ? s : PageStatus.Draft;
        string layoutType = row["layout_type"].ToString();
        string contentJson = row["content"].ToString(); 

        return new Page(
            Convert.ToInt32(row["id"]),
            row["title"].ToString(),
            row["slug"].ToString(),
            status,
            contentJson,
            layoutType,
            Convert.ToDateTime(row["created_at"]),
            Convert.ToDateTime(row["updated_at"])
        );
    }
}
