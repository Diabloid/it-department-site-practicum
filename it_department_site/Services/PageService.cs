using it_department_site.Database;
using it_department_site.Repositories;
using it_department_site.Services;
public class PageService : IPageService
{
    private readonly IPageRepository _pageRepository;

    public PageService(IPageRepository pageRepository)
    {
        _pageRepository = pageRepository;
    }

    public Page? GetPageBySlug(string slug, bool isAdmin)
    {
        var page = _pageRepository.GetBySlug(slug);
        if (page == null) return null;

        // ❗ Тільки адмін бачить чернетки
        if (page.Status == PageStatus.Draft && !isAdmin)
            return null;

        return page;
    }

    public Page? GetPageById(int id) => _pageRepository.GetById(id);

    public List<Page> GetAllPages() => _pageRepository.GetAll();

    public void CreatePage(Page page) => _pageRepository.Create(page);

    public void UpdatePage(Page page) => _pageRepository.Update(page);

    public void DeletePage(int id) => _pageRepository.Delete(id);
}
