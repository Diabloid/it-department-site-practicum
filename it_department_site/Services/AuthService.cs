namespace it_department_site.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;

        public AuthService(IAuthRepository repository)
        {
            _repository = repository;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            return await _repository.ValidateCredentialsAsync(username, password);
        }

        public async Task<int> GetUserIdAsync(string username)
        {
            return await _repository.GetUserIdAsync(username);
        }
    }
}