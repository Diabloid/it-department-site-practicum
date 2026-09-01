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
            var script = await File.ReadAllTextAsync(scriptPath);
            // escape the JSON curly braces as ExecuteSqlRawAsync method treats them as placeholders
            var sanitizedScript = script.Replace("{", "{{").Replace("}", "}}");
            await dbContext.Database.ExecuteSqlRawAsync(sanitizedScript);
        }
    }
}
