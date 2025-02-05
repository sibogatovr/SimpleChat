using Microsoft.EntityFrameworkCore;
using SimpleChat.Data;

namespace SimpleChat.Extensions;

public static class DatabaseExtensions
{
    public static WebApplicationBuilder ConfigureDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Database")
                               ?? throw new InvalidOperationException("Connection string 'Database' not found.");
        builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        return builder;
    }

    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        await using var context = await dbContextFactory.CreateDbContextAsync();

#if DEBUG
        await context.Database.EnsureCreatedAsync();
#else
        await context.Database.MigrateAsync();
#endif
    }
}