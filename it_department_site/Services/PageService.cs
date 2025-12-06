public class PageService : IPageService
{
    private readonly IPageRepository _pageRepository;
    private readonly EncryptionService _encryption;

    public PageService(IPageRepository pageRepository, EncryptionService encryption)
    {
        _pageRepository = pageRepository;
        _encryption = encryption;
    }

    public async Task<Page?> GetPageBySlugAsync(string slug, bool isAdmin)
    {
        var page = await _pageRepository.GetBySlugAsync(slug);
        if (page == null) return null;

        if (page.Status == PageStatus.Draft && !isAdmin)
            return null;

        DecryptContent(page);
        return page;
    }

    public async Task<Page?> GetPageByIdAsync(int id)
    {
        var page = await _pageRepository.GetByIdAsync(id);
        if (page != null)
        {
            DecryptContent(page);
        }
        return page;
    }

    public async Task<List<Page>> GetAllPagesAsync()
    {
        var pages = await _pageRepository.GetAllAsync();
        foreach (var page in pages)
            DecryptContent(page);
        return pages;
    }

    public async Task<List<Page>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var pages = await _pageRepository.GetPagedAsync(pageNumber, pageSize);
        foreach (var page in pages)
            DecryptContent(page);
        return pages;
    }

    public async Task<int> GetTotalAsync(int p) => await _pageRepository.GetTotalAsync(p);

    public async Task<List<Page>> SearchAsync(string query)
    {
        var pages = await _pageRepository.SearchAsync(query);
        foreach (var page in pages)
            DecryptContent(page);
        return pages;
    }

    public async Task<int> CreatePageAsync(Page page)
    {
        EncryptContent(page);
        return await _pageRepository.CreateAsync(page);
    }

    public async Task UpdatePageAsync(Page page)
    {
        EncryptContent(page);
        await _pageRepository.UpdateAsync(page);
    }

    public async Task DeletePageAsync(int id) => await _pageRepository.DeleteAsync(id);

    // Логіка переглядів
    public async Task<bool> HasUserViewedPageAsync(int userId, int pageId)
    {
        return await _pageRepository.HasPageViewAsync(userId, pageId);
    }

    public async Task MarkPageAsViewedAsync(int userId, int pageId)
    {
        await _pageRepository.AddPageViewAsync(userId, pageId);
        await _pageRepository.IncrementViewsAsync(pageId);
    }

    public async Task<int> GetViewsAsync(int pageId)
    {
        return await _pageRepository.GetViewsAsync(pageId);
    }

    // --- Private Methods (Encryption) ---
    // Вони лишаються синхронними, бо це робота з пам'яттю/CPU

    private void EncryptContent(Page page)
    {
        var json = JsonSerializer.Serialize(page.Content);
        // Прибрав зайві логи в консоль
        var encrypted = _encryption.Encrypt(json);
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

            var decrypted = _encryption.Decrypt(page.ContentJson);

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
                page.Content = JsonSerializer.Deserialize<ContentSections>(page.ContentJson) ?? new ContentSections();
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