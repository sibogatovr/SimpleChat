using SimpleChat.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.ConfigureDatabase();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

await app.InitializeDatabaseAsync();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Messages}/{action=Index}/{id?}");

app.Run();