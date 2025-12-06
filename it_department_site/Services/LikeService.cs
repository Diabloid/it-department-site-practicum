namespace it_department_site.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _repo;

        public LikeService(ILikeRepository repo) => _repo = repo;

        public async Task<bool> HasLikedAsync(int userId, int pageId) => await _repo.HasUserLikedAsync(userId, pageId);

        public async Task<bool> ToggleLikeAsync(int userId, int pageId)
        {
            if (await _repo.HasUserLikedAsync(userId, pageId))
            {
                await _repo.RemoveLikeAsync(userId, pageId);
                return false;
            }
            else
            {
                await _repo.AddLikeAsync(userId, pageId);
                return true;
            }
        }

        public async Task<int> GetLikeCountAsync(int pageId) => await _repo.GetLikeCountAsync(pageId);
    }
}