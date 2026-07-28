namespace it_department_site.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _repository;

        public ProfileService(IProfileRepository repository)
        {
            _repository = repository;
        }

        public async Task<User?> GetUserInfoAsync(int userId)
        {
            return await _repository.GetUserInfoAsync(userId);
        }

    }
}