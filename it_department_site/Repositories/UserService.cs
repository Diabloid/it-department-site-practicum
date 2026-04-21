namespace it_department_site.Repositories;

using it_department_site.Data;
using it_department_site.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> FindUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<int> GetUserIdAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        return user?.Id ?? 0;
    }

}