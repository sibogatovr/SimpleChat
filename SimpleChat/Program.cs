using Microsoft.OpenApi.Models;
using SimpleChat.Extensions;
using SimpleChat.Hubs;
using SimpleChat.Services;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureDatabase();
builder.ConfigureLogging();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpleChat API", Version = "v1" });
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SimpleChat.xml"));
});
builder.Services.AddTransient<IMessageService, MessageService>();


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleChat API V1");
});

await app.InitializeDatabaseAsync();
app.UseStaticFiles();
app.MapHub<ChatHub>("/chathub");

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "chat",
    pattern: "chat",
    defaults: new { controller = "Chat", action = "Index" });

app.Run();