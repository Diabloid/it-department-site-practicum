namespace it_department_site.Repositories.Interfaces
{
    public interface IProfileRepository
    {
        Task<User?> GetUserInfoAsync(int userId);
    }
}