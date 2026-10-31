using Microsoft.EntityFrameworkCore;

namespace it_department_site.Infrastructure.Extensions;

public static class DataSourceExtension
{
    public static void ConfigureDatasource(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Якщо вказано реальне віддалене підключення до MySQL (не localhost і не пусте)
        if (!string.IsNullOrWhiteSpace(connectionString) 
            && !connectionString.Contains("localhost", StringComparison.OrdinalIgnoreCase)
            && !connectionString.Contains("127.0.0.1"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));
        }
        else
        {
            // Безпечний fallback для деплою на Render / хмарного тестування без локальної MySQL
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("RenderProductionDb"));
        }
    }
}
