namespace it_department_site.Repositories
{
    // Will be removed as a part of https://github.com/polytech-st/it-department-site/pull/89
    public class PageRepository(IDbConnection db) : IPageRepository
    {
        public async Task IncrementViewsAsync(int pageId)
        {
            await db.ExecuteAsync(
                "UPDATE pages SET Views = Views + 1 WHERE Id = @pageId",
                new { pageId });
        }
    }
}
