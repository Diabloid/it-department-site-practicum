namespace it_department_site.Services
{
    public interface ILikeService
    {
        bool HasLiked(int userId, int pageId);
        bool ToggleLike(int userId, int pageId);
        int GetLikeCount(int pageId);
    }
}
