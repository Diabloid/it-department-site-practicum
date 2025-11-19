using it_department_site.Database;
using it_department_site.Repositories;
using System.Collections.Generic;

namespace it_department_site.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _repo;

        public FavoriteService(IFavoriteRepository repo)
        {
            _repo = repo;
        }

        public void AddToFavorites(int userId, int pageId)
        {
            if (!_repo.IsAlreadyFavorite(userId, pageId))
            {
                _repo.AddToFavorites(userId, pageId);
            }
        }

        public bool IsAlreadyFavorite(int userId, int pageId)
        {
            return _repo.IsAlreadyFavorite(userId, pageId);
        }

        public List<Favorite> GetUserFavorites(int userId)
        {
            return _repo.GetFavoritesByUserId(userId);
        }
        public void RemoveFromFavorites(int userId, int pageId)
        {
            _repo.RemoveFromFavorites(userId, pageId);
        }
        public bool ToggleFavorite(int userId, int pageId)
        {
            if (_repo.IsAlreadyFavorite(userId, pageId))
            {
                _repo.RemoveFromFavorites(userId, pageId);
                return false;
            }
            else
            {
                _repo.AddToFavorites(userId, pageId);
                return true;
            }
        }
    }
}
