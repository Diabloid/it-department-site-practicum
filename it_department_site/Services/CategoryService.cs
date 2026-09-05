namespace it_department_site.Services;

using it_department_site.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class CategoryService(ApplicationDbContext context) : ICategoryService
{
    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await context.Set<Category>().AsNoTracking().ToListAsync();
    }
}
