var builder = WebApplication.CreateBuilder(args);

// Додаємо сервіси
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<dbhelper.Dbhelper>();

var app = builder.Build();

// Обробка помилок
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Маршрут для панелі адміністратора (AdminController)
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin",
    defaults: new { controller = "Admin", action = "Index" }
);

// Маршрут для відображення сторінок за slug (наприклад, /blog-post-1)
app.MapControllerRoute(
    name: "pageBySlug",
    pattern: "{slug}",
    defaults: new { controller = "Pages", action = "Details" }
);

// Основний маршрут за замовчуванням
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pages}/{action=Index}/{id?}"
);

app.Run();
