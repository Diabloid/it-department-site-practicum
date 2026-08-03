using it_department_site.Entities;
using it_department_site.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Services
{
    public class ProfileService(ApplicationDbContext context) : IProfileService
    {
        public async Task<User?> GetUserInfoAsync(int userId)
        {
            return await context.Set<User>()
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new User
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt
                    // Password навмисно НЕ включаємо
                })
                .FirstOrDefaultAsync();
        }
    }
}