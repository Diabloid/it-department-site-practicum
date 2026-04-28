namespace it_department_site.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string username, string password);

    }
}
