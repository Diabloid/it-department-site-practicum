namespace it_department_site.Services;

using it_department_site.Entities;
using it_department_site.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class GraduateService : IGraduateService
{
    private readonly ApplicationDbContext _context;

    public GraduateService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Graduate>> GetAllAsync()
    {
        return await _context.Graduates.ToListAsync();
    }

    public async Task<Graduate?> GetByIdAsync(int id)
    {
        return await _context.Graduates.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task CreateAsync(Graduate graduate)
    {
        _context.Graduates.Add(graduate);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, Graduate graduate)
{
    var existing = GetByIdAsync(id).Result;
    if (existing == null) return;

    existing.FirstName = graduate.FirstName;
    existing.LastName = graduate.LastName;
    existing.GraduationYear = graduate.GraduationYear;
    existing.Specialization = graduate.Specialization;
    existing.JobDescription = graduate.JobDescription;
    existing.Testimonial = graduate.Testimonial;

    // ← зберігаємо старе фото якщо нове не завантажили
    if (!string.IsNullOrEmpty(graduate.PhotoUrl))
        existing.PhotoUrl = graduate.PhotoUrl;

    await _context.SaveChangesAsync();
}

    public async Task DeleteAsync(int id)
    {
        var existing = GetByIdAsync(id).Result;
        if (existing == null) return;

        _context.Graduates.Remove(existing);
        await _context.SaveChangesAsync();
    }
}