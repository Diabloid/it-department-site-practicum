namespace it_department_site.Repositories;

using it_department_site.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IDbConnection _db;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
        _db = context.Database.GetDbConnection();
    }
    
    public async Task<User?> FindUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = await _db.QueryAsync<User>("SELECT id, username, email, created_at FROM users ORDER BY id");
        return users;
    }

}
