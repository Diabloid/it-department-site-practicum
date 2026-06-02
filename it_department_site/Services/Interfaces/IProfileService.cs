namespace it_department_site.Services.Interfaces
{
    public interface IProfileService
    {
        Task<User?> GetUserInfoAsync(int userId);
    }
}