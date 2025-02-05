using Microsoft.Extensions.Logging.Abstractions;

namespace SimpleChat;

public static class LogHost
{
    public static IServiceProvider? ServiceProvider { get; set; }
    public static ILogger<T> GetLogger<T>() => ServiceProvider?.GetRequiredService<ILogger<T>>() ?? new NullLogger<T>();
}