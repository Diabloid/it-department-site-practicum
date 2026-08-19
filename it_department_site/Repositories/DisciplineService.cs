using it_department_site.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Repositories;

public class DisciplineService : IDisciplineService
{
    private readonly ApplicationDbContext _context;

    public DisciplineService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Discipline>> GetAllAsync()
    {
        return await _context.Disciplines
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<Discipline?> GetByIdAsync(int id)
    {
        return await _context.Disciplines.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task CreateAsync(Discipline discipline)
    {
        _context.Disciplines.Add(discipline);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, Discipline discipline)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) return;

        existing.Name = discipline.Name;
        existing.Description = discipline.Description;
        existing.SortOrder = discipline.SortOrder;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) return;

        _context.Disciplines.Remove(existing);
        await _context.SaveChangesAsync();
    }
}
