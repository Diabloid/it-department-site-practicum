namespace it_department_site.Repositories
{
    public interface ILikeRepository
    {
        bool HasUserLiked(int userId, int pageId);
        void AddLike(int userId, int pageId);
        void RemoveLike(int userId, int pageId);
        int GetLikeCount(int pageId);
    }
}
