using DotNetEnv;
using it_department_site.Infrastructure.Extensions;
using it_department_site.Infrastructure.Resilience;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.RateLimiting;
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

            // Підтримка динамічного порту Render / Cloud хмарного хостингу
            var port = Environment.GetEnvironmentVariable("PORT");
            if (!string.IsNullOrEmpty(port))
            {
                builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
            }
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

            // Вьюхи для area "Admin" лежать у папці Admin/ у корені проєкту
            builder.Services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/Admin/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Admin/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");

                options.ViewLocationFormats.Add("/Public/{1}/{0}.cshtml");
                options.ViewLocationFormats.Add("/Public/Pages/{0}.cshtml");
            });

            builder.Services.AddSession();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();

            // Модуль 3: Caching (In-memory caching для зниження навантаження на БД)
            builder.Services.AddMemoryCache();

            // Модуль 3: Відмовостійкість (Resilience Service: Polly Retry, Circuit Breaker, Timeout)
            builder.Services.AddSingleton<IResilienceService, ResilienceService>();

            // Модуль 3: Rate Limiting (Обмеження запитів до API: 30 запитів на хвилину, повертає 429)
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddFixedWindowLimiter("api", limiter =>
                {
                    limiter.PermitLimit = 30;
                    limiter.Window = TimeSpan.FromMinutes(1);
                    limiter.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    limiter.QueueLimit = 0;
                });
            });

            builder.Services.ConfigureDatasource(builder.Configuration);

            // Реєстрація репозиторіїв та сервісів
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

            var defaultConn = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddQuartz(q =>
            {
                if (!string.IsNullOrWhiteSpace(defaultConn))
                {
                    q.UsePersistentStore(s =>
                    {
                        s.UseNewtonsoftJsonSerializer();
                        s.UseMySqlConnector(mySqlOptions =>
                        {
                            mySqlOptions.ConnectionString = defaultConn;
                        });
                    });
                }
            });

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

            // Модуль 3: Підключення Rate Limiting middleware
            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");
            app.UseSession();

            // Модуль 3: Мапінг REST API контролерів
            app.MapControllers();

            // Маршрути MVC
            app.MapControllerRoute(
                name: "zones",
                pattern: "{area:exists}/{controller}/{action=Index}/{id?}"
            );

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
                pattern: "{controller=HomePage}/{action=Index}/{id?}"
            );

            await app.RunAsync();
        }
    }
}
