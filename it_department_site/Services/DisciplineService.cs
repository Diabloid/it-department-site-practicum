using it_department_site.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Services;

public class DisciplineService(ApplicationDbContext context) : IDisciplineService
{
    public async Task<IEnumerable<Discipline>> GetAllAsync()
    {
        return await context.Disciplines
            .Include(d => d.ResourceLinks.OrderBy(l => l.Order))
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<Discipline?> GetByIdAsync(int id)
    {
        return await context.Disciplines
            .Include( d => d.Employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName))
            .Include(d => d.ResourceLinks.OrderBy(l => l.Order))
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task CreateAsync(Discipline discipline)
    {
        context.Disciplines.Add(discipline);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, Discipline discipline)
    {
        var existing = await context.Disciplines
            .Include(d => d.ResourceLinks)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (existing == null) return;

        existing.Name = discipline.Name;
        existing.Description = discipline.Description;
        existing.SortOrder = discipline.SortOrder;
        existing.IsRequired = discipline.IsRequired;

        context.RemoveRange(existing.ResourceLinks);
        existing.ResourceLinks = discipline.ResourceLinks
            .Select(l => new DisciplineResourceLink { Title = l.Title, Url = l.Url, Order = l.Order, DisciplineId = id })
            .ToList();

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        // REFACTOR_HINT: GetByIdAsync includes unnecessary entities
        var existing = await GetByIdAsync(id);
        if (existing == null) return;

        context.Disciplines.Remove(existing);
        await context.SaveChangesAsync();
    }

    public async Task<List<Discipline>> GetPagedAsync(bool includeRequired, string? name, int pageNumber, int pageSize)
    {
        return await FilteredQuery(includeRequired, name)
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Name)
            .Skip(Math.Max(pageNumber - 1, 0) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalPagesCountAsync(bool includeRequired, string? name, int pageSize)
    {
        if (pageSize < 1) pageSize = 1;

        var totalCount = await FilteredQuery(includeRequired, name).CountAsync();
        return (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    private IQueryable<Discipline> FilteredQuery(bool includeRequired, string? name)
    {
        var query = context.Set<Discipline>()
            .AsNoTracking()
            .Where(d => d.IsRequired == includeRequired);

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(d => EF.Functions.Like(d.Name, $"%{name}%"));
        }

        return query;
    }
}
