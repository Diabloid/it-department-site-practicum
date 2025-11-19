using it_department_site.Database;
using System.Collections.Generic;

namespace it_department_site.Services
{
    public interface IFavoriteService
    {
        void AddToFavorites(int userId, int pageId);
        bool IsAlreadyFavorite(int userId, int pageId);
        List<Favorite> GetUserFavorites(int userId);
        void RemoveFromFavorites(int userId, int pageId);
        bool ToggleFavorite(int userId, int pageId);
    }
}
