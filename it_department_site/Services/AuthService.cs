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
            var user = await _repository.FindUserByUsernameAsync(username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user;
            }
            else
            {
                return null;
            }

        }

        public async Task<int> GetUserIdAsync(string username)
        {
            return await _repository.GetUserIdAsync(username);
        }
    }
}