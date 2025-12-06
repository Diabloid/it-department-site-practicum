using it_department_site.Database;
using it_department_site.Repositories;

namespace it_department_site.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;

        public AuthService(IAuthRepository repository)
        {
            _repository = repository;
        }

        public User? Login(string username, string password)
        {
            return _repository.ValidateCredentials(username, password);
        }

        public int GetUserId(string username)
        {
            return _repository.GetUserId(username);
        }
    }
}
