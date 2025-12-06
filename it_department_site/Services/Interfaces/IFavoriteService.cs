namespace it_department_site.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task AddToFavoritesAsync(int userId, int pageId);
        Task<bool> IsAlreadyFavoriteAsync(int userId, int pageId);
        Task<List<Favorite>> GetUserFavoritesAsync(int userId);
        Task RemoveFromFavoritesAsync(int userId, int pageId);
        Task<bool> ToggleFavoriteAsync(int userId, int pageId);
    }
}