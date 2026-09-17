using it_department_site.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Services;

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
            .Include(d => d.ResourceLinks.OrderBy(l => l.Order))
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<Discipline?> GetByIdAsync(int id)
    {
        return await _context.Disciplines
            .Include(d => d.ResourceLinks.OrderBy(l => l.Order))
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task CreateAsync(Discipline discipline)
    {
        _context.Disciplines.Add(discipline);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, Discipline discipline)
    {
        var existing = await _context.Disciplines
            .Include(d => d.ResourceLinks)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (existing == null) return;

        existing.Name = discipline.Name;
        existing.Description = discipline.Description;
        existing.SortOrder = discipline.SortOrder;
        existing.IsRequired = discipline.IsRequired;

        _context.RemoveRange(existing.ResourceLinks);
        existing.ResourceLinks = discipline.ResourceLinks
            .Select(l => new DisciplineResourceLink { Title = l.Title, Url = l.Url, Order = l.Order, DisciplineId = id })
            .ToList();

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) return;

        _context.Disciplines.Remove(existing);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Discipline>> GetPagedAsync(int pageNumber, int pageSize, string? name)
    {
        var query = _context.Disciplines.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(d => EF.Functions.Like(d.Name, $"%{name}%"));
        }

        return await query
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}