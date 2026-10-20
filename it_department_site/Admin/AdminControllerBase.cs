using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace it_department_site.Admin
{
    /// <summary>
    /// Базований контролер для area "Admin".
    /// Доступний лише користувачам з роллю Admin.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public abstract class AdminControllerBase : Controller
    {
    }
}
