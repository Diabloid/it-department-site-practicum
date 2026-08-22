using Microsoft.EntityFrameworkCore;

namespace it_department_site.Infrastructure.Extensions;

public static class DataSourceExtension
{
    public static void ConfigureDatasource(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))));
    }
}
