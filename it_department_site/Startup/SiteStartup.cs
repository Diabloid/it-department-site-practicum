using DotNetEnv;
using it_department_site.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Quartz;
using System.Security.Claims;

namespace it_department_site.Startup
{
    public class SiteStartup(string[] args) : IStartup
    {
        public async Task StartAsync()
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<GoogleDriveOptions>(
                builder.Configuration.GetSection(GoogleDriveOptions.SectionName));

            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    options.Instance = "https://login.microsoftonline.com/";
                    options.TenantId = builder.Configuration["EntraId:TenantId"];
                    options.ClientId = builder.Configuration["EntraId:ClientId"];
                    options.ClientSecret = builder.Configuration["EntraId:ClientSecret"];
                    options.CallbackPath = "/signin-oidc";
                    options.ResponseType = "code";

                    options.EventsType = typeof(EntraIdAuthenticationEvents);
                });

            builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "IPZAuthCookie";
                options.LoginPath = "/login";
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();

            builder.Services.ConfigureDatasource(builder.Configuration);
            // 4. Реєстрація Репозиторіїв та Сервісів (Всі Scoped)
            builder.Services.AddScoped<IPageService, PageService>();
            builder.Services.AddScoped<IMenuService, MenuService>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IGraduateService, GraduateService>();
            builder.Services.AddScoped<IDisciplineService, DisciplineService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<ISiteSettingsService, SiteSettingsService>();

            builder.Services.AddScoped<IProfileService, ProfileService>();

            builder.Services.AddScoped<EntraIdAuthenticationEvents>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddScoped<IPartnerService, PartnerService>();
            builder.Services.AddScoped<IMainSliderService, MainSliderService>();

            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IOrganizationInfoService, OrganizationInfoService>();
            builder.Services.AddSingleton<IGoogleDriveService, GoogleDriveService>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();


            builder.Services.AddQuartz(q =>
            {
                q.UsePersistentStore(s =>
                {
                    // Використовуємо JSON для серіалізації параметрів завдання
                    s.UseNewtonsoftJsonSerializer();

                    s.UseMySqlConnector(mySqlOptions =>
                    {
                        mySqlOptions.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                    });
                });
            });

            // Додаємо Quartz як фоновий сервіс, який чекатиме завершення задач при вимкненні програми
            builder.Services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // Хто ти?
            app.UseAuthorization(); // Що тобі можна?

            app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");

            app.UseSession();

            // Маршрути
            app.MapControllerRoute(
                name: "admin",
                pattern: "Admin",
                defaults: new { controller = "Admin", action = "Index" }
            );

            app.MapControllerRoute(
                name: "disciplines",
                pattern: "Disciplines/{action=Index}/{id?}",
                defaults: new { controller = "Disciplines" });

            app.MapControllerRoute(
                name: "proudOfYou",
                pattern: "proudofyou",
                defaults: new { controller = "Graduates", action = "ProudOfYou" });

            app.MapControllerRoute(
                name: "graduatesTimeline",
                pattern: "timeline",
                defaults: new { controller = "Graduates", action = "Timeline" });

            app.MapControllerRoute(
                name: "graduatesDetails",
                pattern: "Graduates/Details/{id:int}",
                defaults: new { controller = "Graduates", action = "Details" });

            app.MapControllerRoute(
                name: "employees",
                pattern: "Employees",
                defaults: new { controller = "Employees", action = "Index" });

            app.MapControllerRoute(
                name: "pageBySlug",
                pattern: "{slug}",
                defaults: new { controller = "Pages", action = "Details" }
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Pages}/{action=Index}/{id?}"
            );

            await app.RunAsync();
        }
    }
}
