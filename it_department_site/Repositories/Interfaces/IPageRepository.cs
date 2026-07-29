namespace it_department_site.Repositories.Interfaces
{
    public interface IPageRepository
    {
        Task IncrementViewsAsync(int pageId);
    }
}
