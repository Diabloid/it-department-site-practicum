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

        public bool Login(string username, string password)
        {
            // колись тут може бути хешування пароля або перевірка статусу
            return _repository.ValidateCredentials(username, password);
        }
    }
}
