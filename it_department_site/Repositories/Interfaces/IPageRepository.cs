namespace it_department_site.Repositories.Interfaces
{
    public interface IPageRepository
    {
        Task<bool> HasPageViewAsync(int userId, int pageId);

        Task AddPageViewAsync(int userId, int pageId);

        Task IncrementViewsAsync(int pageId);
    }
}