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

        public async Task<User?> GetUserInfoAsync(int userId)
        {
            return await _repository.GetUserInfoAsync(userId);
        }

        public async Task<List<Favorite>> GetFavoritesAsync(int userId)
        {
            return await _favoriteRepository.GetFavoritesByUserIdAsync(userId);
        }
    }
}