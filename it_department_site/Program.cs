using it_department_site.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddCookie(options=>
{
    options.Cookie.Name = "IPZAuthCookie";
    options.LoginPath = "/login";
});

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// 3. Реєстрація підключення до БД (Dapper)
builder.Services.AddScoped<IDbConnection>(sp =>
    new MySqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));
// 4. Реєстрація Репозиторіїв та Сервісів (Всі Scoped)
builder.Services.AddScoped<IPageRepository, PageRepository>();
builder.Services.AddScoped<IPageService, PageService>();

builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuService, MenuService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGraduateService, GraduateService>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddScoped<ISiteSettingsRepository, SiteSettingsRepository>();
builder.Services.AddScoped<ISiteSettingsService, SiteSettingsService>();

builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ILikeService, LikeService>();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<EncryptionService>();

builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
builder.Services.AddScoped<IPartnerService, PartnerService>();

builder.Services.AddScoped<IPostRepository, PostRepository>();
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
app.UseAuthorization();  // Що тобі можна?

app.UseSession();

// Маршрути
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin",
    defaults: new { controller = "Admin", action = "Index" }
);

app.MapControllerRoute(
    name: "pageBySlug",
    pattern: "{slug}",
    defaults: new { controller = "Pages", action = "Details" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pages}/{action=Index}/{id?}"
);

app.Run();
