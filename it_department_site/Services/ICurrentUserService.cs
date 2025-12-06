namespace it_department_site.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        bool IsAdmin { get; }
        bool IsAuthenticated { get; }
    }
}