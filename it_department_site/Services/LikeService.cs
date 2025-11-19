using it_department_site.Repositories;

namespace it_department_site.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _repo;

        public LikeService(ILikeRepository repo) => _repo = repo;

        public bool HasLiked(int userId, int pageId) => _repo.HasUserLiked(userId, pageId);

        public bool ToggleLike(int userId, int pageId)
        {
            if (_repo.HasUserLiked(userId, pageId))
            {
                _repo.RemoveLike(userId, pageId);
                return false;
            }
            else
            {
                _repo.AddLike(userId, pageId);
                return true;
            }
        }

        public int GetLikeCount(int pageId) => _repo.GetLikeCount(pageId);
    }
}
