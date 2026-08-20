namespace it_department_site.Services;

using it_department_site.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class UserService(ApplicationDbContext context) : IUserService
{
    
    public async Task<User?> FindUserByUsernameAsync(string username)
    {
        return await context.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await context.Set<User>().AsNoTracking().ToListAsync();
    }

}
