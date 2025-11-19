using it_department_site.Database;
using System.Collections.Generic;

namespace it_department_site.Repositories
{
    public interface IFavoriteRepository
    {
        bool IsAlreadyFavorite(int userId, int pageId);
        void AddToFavorites(int userId, int pageId);
        List<Favorite> GetFavoritesByUserId(int userId);
        void RemoveFromFavorites(int userId, int pageId);
    }
}
