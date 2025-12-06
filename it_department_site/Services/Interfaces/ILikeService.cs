namespace it_department_site.Services.Interfaces
{
    public interface ILikeService
    {
        Task<bool> HasLikedAsync(int userId, int pageId);
        Task<bool> ToggleLikeAsync(int userId, int pageId);
        Task<int> GetLikeCountAsync(int pageId);
    }
}