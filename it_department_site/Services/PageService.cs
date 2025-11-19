using it_department_site.Database;
using it_department_site.Repositories;
using it_department_site.Services;
using NuGet.Protocol.Core.Types;
using System.Text.Json;

public class PageService : IPageService
{
    private readonly IPageRepository _pageRepository;
    private readonly EncryptionService _encryption;

    public PageService(IPageRepository pageRepository, EncryptionService encryption)
    {
        _pageRepository = pageRepository;
        _encryption = encryption;
    }

    public Page? GetPageBySlug(string slug, bool isAdmin)
    {
        var page = _pageRepository.GetBySlug(slug);
        if (page == null) return null;

        if (page.Status == PageStatus.Draft && !isAdmin)
            return null;

        // Дешифруємо вміст
        DecryptContent(page);
        return page;
    }

    public Page GetPageById(int id)
    {
        var page = _pageRepository.GetById(id);
        if (page != null)
        {
            DecryptContent(page);
        }
        return page;
    }

    public List<Page> GetAllPages()
    {
        var pages = _pageRepository.GetAll();
        foreach (var page in pages)
            DecryptContent(page);
        return pages;
    }

    public List<Page> GetPaged(int pageNumber, int pageSize)
    {
        var pages = _pageRepository.GetPaged(pageNumber, pageSize);
        foreach (var page in pages)
            DecryptContent(page);
        return pages;
    }

    public int GetTotal(int p) => _pageRepository.GetTotal(p);

    public List<Page> Search(string query)
    {
        var pages = _pageRepository.Search(query);
        foreach (var page in pages)
            DecryptContent(page);
        return pages;
    }

    public int CreatePage(Page page)
    {
        EncryptContent(page);
        return _pageRepository.Create(page);   
    }

    public void UpdatePage(Page page)
    {
        EncryptContent(page);
        _pageRepository.Update(page);
    }

    public void DeletePage(int id) => _pageRepository.Delete(id);

    // 🔒 Метод шифрування вмісту перед збереженням
    private void EncryptContent(Page page)
    {
        var json = JsonSerializer.Serialize(page.Content);

        Console.WriteLine("📝 Content to encrypt:");
        Console.WriteLine(json);

        var encrypted = _encryption.Encrypt(json);

        Console.WriteLine("🔐 Encrypted result:");
        Console.WriteLine(encrypted);

        page.ContentJson = encrypted;
    }

    // 🔓 Метод дешифрування після зчитування
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
            // fallback для незашифрованих записів
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
    public bool HasUserViewedPage(int userId, int pageId)
    {
        return _pageRepository.HasPageView(userId, pageId);
    }
    public void MarkPageAsViewed(int userId, int pageId)
    {
        _pageRepository.AddPageView(userId, pageId);
        _pageRepository.IncrementViews(pageId);
    }
    public int GetViews(int pageId)
    {
        return _pageRepository.GetViews(pageId);
    }
}
