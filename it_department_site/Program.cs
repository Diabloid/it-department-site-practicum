var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Налаштування маршруту для DbController і методу Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Db}/{action=Index}/{id?}"); // Замість HomeController ставимо DbController

app.Run();
