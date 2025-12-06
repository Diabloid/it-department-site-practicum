namespace it_department_site.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _repo;

        public FavoriteService(IFavoriteRepository repo)
        {
            _repo = repo;
        }

        public async Task AddToFavoritesAsync(int userId, int pageId)
        {
            if (!await _repo.IsAlreadyFavoriteAsync(userId, pageId))
            {
                await _repo.AddToFavoritesAsync(userId, pageId);
            }
        }

        public async Task<bool> IsAlreadyFavoriteAsync(int userId, int pageId)
        {
            return await _repo.IsAlreadyFavoriteAsync(userId, pageId);
        }

        public async Task<List<Favorite>> GetUserFavoritesAsync(int userId)
        {
            return await _repo.GetFavoritesByUserIdAsync(userId);
        }

        public async Task RemoveFromFavoritesAsync(int userId, int pageId)
        {
            await _repo.RemoveFromFavoritesAsync(userId, pageId);
        }

        public async Task<bool> ToggleFavoriteAsync(int userId, int pageId)
        {
            if (await _repo.IsAlreadyFavoriteAsync(userId, pageId))
            {
                await _repo.RemoveFromFavoritesAsync(userId, pageId);
                return false;
            }
            else
            {
                await _repo.AddToFavoritesAsync(userId, pageId);
                return true;
            }
        }
    }
}