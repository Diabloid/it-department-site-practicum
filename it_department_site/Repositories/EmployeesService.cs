namespace it_department_site.Repositories;

using it_department_site.Entities;
using it_department_site.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;

    public EmployeeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees.ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task CreateAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, Employee employee)
{
    var existing = GetByIdAsync(id).Result;
    if (existing == null) return;

    existing.FirstName = employee.FirstName;
    existing.LastName = employee.LastName;
    existing.PatronymicName = employee.PatronymicName;
    existing.Specialization = employee.Specialization;
    existing.Bio = employee.Bio;
    existing.Email = employee.Email;

    // ← зберігаємо старе фото якщо нове не завантажили
    if (!string.IsNullOrEmpty(employee.PhotoUrl))
        existing.PhotoUrl = employee.PhotoUrl;

    await _context.SaveChangesAsync();
}

    public async Task DeleteAsync(int id)
    {
        var existing = GetByIdAsync(id).Result;
        if (existing == null) return;

        _context.Employees.Remove(existing);
        await _context.SaveChangesAsync();
    }
}