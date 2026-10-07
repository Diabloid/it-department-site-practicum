using Microsoft.EntityFrameworkCore;

namespace it_department_site.Services
{
    public class PageService(ApplicationDbContext dbContext) : IPageService
    {
        public async Task<Page?> GetPageBySlugAsync(string slug, bool isAdmin)
        {
            var page = await dbContext.Set<Page>().AsNoTracking().FirstOrDefaultAsync(page => page.Slug == slug);
            if (page == null) return null;

            if (page.Status == PageStatus.Draft && !isAdmin)
                return null;

            DeserializeContent(page);
            return page;
        }

        public async Task<Page?> GetPageByIdAsync(int id)
        {
            var page = await dbContext.Set<Page>().AsNoTracking().FirstOrDefaultAsync(page => page.Id == id);
            if (page != null)
            {
                DeserializeContent(page);
            }

            return page;
        }

        public async Task<List<Page>> GetAllPagesAsync()
        {
            var pages = await dbContext.Set<Page>().AsNoTracking().ToListAsync();
            foreach (var page in pages)
                DeserializeContent(page);
            return pages;
        }

        public async Task<List<Page>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var pages = await dbContext.Set<Page>().AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var page in pages)
                DeserializeContent(page);
            return pages;
        }

        public async Task<int> GetTotalPagesAsync(int pageSize)
        {
            var totalPagesCount = await dbContext.Set<Page>().CountAsync();
            return (int)Math.Ceiling((double)totalPagesCount / pageSize);
        }

        public async Task<int> CreatePageAsync(Page page)
        {
            SerializeContent(page);
            await dbContext.Set<Page>().AddAsync(page);
            await dbContext.SaveChangesAsync();
            return page.Id;
        }

        public async Task UpdatePageAsync(Page page)
        {
            SerializeContent(page);

            dbContext.Set<Page>().Update(page);

            dbContext.Entry(page).Property(p => p.ViewsCount).IsModified = false;

            dbContext.Entry(page).Property(p => p.CreatedAt).IsModified = false;

            await dbContext.SaveChangesAsync();
        }

        public async Task DeletePageAsync(int id)
        {
            await dbContext.Set<Page>()
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task IncrementViewsAsync(int pageId)
        {
            await dbContext.Pages
                .Where(p => p.Id == pageId)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    p => p.ViewsCount,
                    p => p.ViewsCount + 1));
        }

        // --- Private Methods (JSON serialization, no encryption) ---

        private void SerializeContent(Page page)
        {
            page.ContentJson = JsonSerializer.Serialize(page.Content);
        }

        private void DeserializeContent(Page page)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(page.ContentJson))
                {
                    page.Content = new ContentSections();
                    return;
                }

                page.Content = JsonSerializer.Deserialize<ContentSections>(page.ContentJson) ?? new ContentSections();
            }
            catch (JsonException)
            {
                page.Content = new ContentSections();
            }
        }
    }
}