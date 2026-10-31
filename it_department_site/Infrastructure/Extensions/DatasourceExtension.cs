using Microsoft.EntityFrameworkCore;

namespace it_department_site.Infrastructure.Extensions;

public static class DataSourceExtension
{
    public static void ConfigureDatasource(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=localhost;Port=3306;Database=it_department_db;User ID=root;Password=;";

        // Використання явної версії MySQL (8.0.36) запобігає падінню контейнера під час старту через спроби AutoDetect
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));
    }
}
