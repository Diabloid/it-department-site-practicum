using it_department_site.Database;
using it_department_site.Repositories;
using System;
using System.Collections.Generic;

namespace it_department_site.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _repository;
        private readonly IFavoriteRepository _favoriteRepository;

        public ProfileService(IProfileRepository repository, IFavoriteRepository favoriteRepository)
        {
            _repository = repository;
            _favoriteRepository = favoriteRepository;
        }

        public User? GetUserInfo(int userId)
        {
            return _repository.GetUserInfo(userId);
        }

        public List<Favorite> GetFavorites(int userId)
        {
            return _favoriteRepository.GetFavoritesByUserId(userId);
        }
    }
}
