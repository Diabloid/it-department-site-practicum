using it_department_site.Database;
using System;
using System.Collections.Generic;

namespace it_department_site.Services
{
    public interface IProfileService
    {
        User? GetUserInfo(int userId);
        List<Favorite> GetFavorites(int userId);
    }
}
