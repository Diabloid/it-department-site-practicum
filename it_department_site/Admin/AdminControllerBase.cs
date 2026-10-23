using Microsoft.AspNetCore.Authorization;

namespace it_department_site.Admin
{
    [Area("Admin")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public abstract class AdminControllerBase : Controller
    {
    }
}
