namespace it_department_site.Repositories
{
    // Will be removed as a part of https://github.com/polytech-st/it-department-site/pull/89
    public class PageRepository(IDbConnection db) : IPageRepository
    {
        public async Task<bool> HasPageViewAsync(int userId, int pageId)
        {
            int? result = await db.QueryFirstOrDefaultAsync<int?>(
                "SELECT 1 FROM page_views WHERE UserId = @userId AND PageId = @pageId LIMIT 1",
                new { userId, pageId });
            return result.HasValue;
        }

        public async Task AddPageViewAsync(int userId, int pageId)
        {
            await db.ExecuteAsync(
                "INSERT INTO page_views (UserId, PageId) VALUES (@userId, @pageId)",
                new { userId, pageId });
        }

        public async Task IncrementViewsAsync(int pageId)
        {
            await db.ExecuteAsync(
                "UPDATE pages SET Views = Views + 1 WHERE Id = @pageId",
                new { pageId });
        }
    }
}