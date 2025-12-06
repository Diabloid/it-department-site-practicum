namespace it_department_site.Repositories.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<bool> IsAlreadyFavoriteAsync(int userId, int pageId);
        Task AddToFavoritesAsync(int userId, int pageId);
        Task<List<Favorite>> GetFavoritesByUserIdAsync(int userId);
        Task RemoveFromFavoritesAsync(int userId, int pageId);
    }
}