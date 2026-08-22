using it_department_site.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Startup
{
    public class DemoDataSeedStartup(string[] args) : IStartup
    {
        public async Task StartAsync()
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.ConfigureDatasource(builder.Configuration);
            var app = builder.Build();
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var projectRoot = Directory.GetCurrentDirectory();
            var scriptPath = Path.Combine(projectRoot, "Docker", "init.sql");
            var script = File.ReadAllText(scriptPath);
            await dbContext.Database.ExecuteSqlRawAsync(script);
        }
    }
}
