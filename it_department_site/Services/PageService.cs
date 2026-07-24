using it_department_site.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Services
{
    public class PageService(
        ApplicationDbContext dbContext,
        IPageRepository pageRepository,
        EncryptionService encryption) : IPageService
    {
        public async Task<Page?> GetPageBySlugAsync(string slug, bool isAdmin)
        {
            var page = await dbContext.Set<Page>().AsNoTracking().FirstOrDefaultAsync(page => page.Slug == slug);
            if (page == null) return null;

            if (page.Status == PageStatus.Draft && !isAdmin)
                return null;

            DecryptContent(page);
            return page;
        }

        public async Task<Page?> GetPageByIdAsync(int id)
        {
            var page = await dbContext.Set<Page>().AsNoTracking().FirstOrDefaultAsync(page => page.Id == id);
            if (page != null)
            {
                DecryptContent(page);
            }

            return page;
        }

        public async Task<List<Page>> GetAllPagesAsync()
        {
            var pages = await dbContext.Set<Page>().AsNoTracking().ToListAsync();
            foreach (var page in pages)
                DecryptContent(page);
            return pages;
        }

        public async Task<List<Page>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var pages = await dbContext.Set<Page>().AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var page in pages)
                DecryptContent(page);
            return pages;
        }

        public async Task<int> GetTotalPagesAsync(int pageSize)
        {
            var totalPagesCount = await dbContext.Set<Page>().CountAsync();
            return (int)Math.Ceiling((double)totalPagesCount / pageSize);
        }

        public async Task<int> CreatePageAsync(Page page)
        {
            EncryptContent(page);
            await dbContext.Set<Page>().AddAsync(page);
            await dbContext.SaveChangesAsync();
            return page.Id;
        }

        public async Task UpdatePageAsync(Page page)
        {
            EncryptContent(page);
            await dbContext.Set<Page>().AddAsync(page);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeletePageAsync(int id)
        {
            await dbContext.Set<Page>()
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync();
        }

        // Логіка переглядів
        public async Task<bool> HasUserViewedPageAsync(int userId, int pageId)
        {
            return await pageRepository.HasPageViewAsync(userId, pageId);
        }

        public async Task MarkPageAsViewedAsync(int userId, int pageId)
        {
            await pageRepository.AddPageViewAsync(userId, pageId);
            await pageRepository.IncrementViewsAsync(pageId);
        }

        public async Task<int> GetViewsAsync(int id)
        {
            return await dbContext.Set<Page>().AsNoTracking()
                .Where(page => page.Id == id)
                .Select(page => page.ViewsCount)
                .SingleOrDefaultAsync();
        }

        // --- Private Methods (Encryption) ---
        // Вони лишаються синхронними, бо це робота з пам'яттю/CPU

        private void EncryptContent(Page page)
        {
            var json = JsonSerializer.Serialize(page.Content);
            var encrypted = encryption.Encrypt(json);
            page.ContentJson = encrypted;
        }

        private void DecryptContent(Page page)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(page.ContentJson))
                {
                    page.Content = new ContentSections();
                    return;
                }

                var decrypted = encryption.Decrypt(page.ContentJson);

                if (string.IsNullOrWhiteSpace(decrypted))
                {
                    page.Content = new ContentSections();
                    return;
                }

                page.Content = JsonSerializer.Deserialize<ContentSections>(decrypted) ?? new ContentSections();
            }
            catch (FormatException)
            {
                try
                {
                    page.Content = JsonSerializer.Deserialize<ContentSections>(page.ContentJson) ??
                                   new ContentSections();
                }
                catch
                {
                    page.Content = new ContentSections();
                }
            }
            catch (JsonException)
            {
                page.Content = new ContentSections();
            }
        }
    }
}
