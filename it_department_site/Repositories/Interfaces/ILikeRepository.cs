namespace it_department_site.Repositories.Interfaces
{
    public interface ILikeRepository
    {
        Task<bool> HasUserLikedAsync(int userId, int pageId);
        Task AddLikeAsync(int userId, int pageId);
        Task RemoveLikeAsync(int userId, int pageId);
        Task<int> GetLikeCountAsync(int pageId);
    }
}