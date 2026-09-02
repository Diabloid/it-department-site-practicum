using it_department_site.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Startup
{
    public class MigrationStartup(string[] args) : IStartup
    {
        public async Task StartAsync()
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.ConfigureDatasource(builder.Configuration);
            var app = builder.Build();
            using var scope = app.Services.CreateScope();
            var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await applicationDbContext.Database.MigrateAsync();
        }
    }
}
