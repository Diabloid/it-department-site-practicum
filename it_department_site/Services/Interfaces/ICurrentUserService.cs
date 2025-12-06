namespace it_department_site.Services.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        bool IsAdmin { get; }
        bool IsAuthenticated { get; }
    }
}