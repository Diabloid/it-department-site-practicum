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
        return await context.Set<User>().AsNoTracking().Include(u => u.Employee).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await context.Set<User>().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task CreateAsync(User user, string plainPassword)
    {
        user.Password = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        user.CreatedAt = DateTime.UtcNow;
        context.Set<User>().Add(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, User user, string? plainPassword)
    {
        var existing = await context.Set<User>().FirstOrDefaultAsync(u => u.Id == id);
        if (existing == null) return;

        existing.Username = user.Username;
        existing.Email = user.Email;
        existing.EmployeeId = user.EmployeeId;
        existing.Roles = user.Roles;

        if (!string.IsNullOrWhiteSpace(plainPassword))
        {
            existing.Password = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Employee>> GetAvailableEmployeesAsync(int? excludeUserId = null)
    {
        var takenEmployeeIds = await context.Set<User>()
            .Where(u => u.EmployeeId != null && u.Id != (excludeUserId ?? 0))
            .Select(u => u.EmployeeId!.Value)
            .ToListAsync();

        return await context.Set<Employee>()
            .AsNoTracking()
            .Where(e => !takenEmployeeIds.Contains(e.Id))
            .OrderBy(e => e.LastName)
            .ToListAsync();
    }
}