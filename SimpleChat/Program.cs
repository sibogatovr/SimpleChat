using Microsoft.OpenApi.Models;
using SimpleChat.Extensions;
using SimpleChat.Hubs;
using SimpleChat.Services;
using SimpleChat.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddTransient<IMessageRepository, MessageRepository>();


var app = builder.Build();

var messageRepository = app.Services.GetRequiredService<IMessageRepository>();
await messageRepository.EnsureDatabaseCreatedAsync();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleChat API V1");
});
app.UseStaticFiles();
app.MapHub<ChatHub>("/chathub");

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "chat",
    pattern: "chat",
    defaults: new { controller = "Chat", action = "Index" });

app.Run();