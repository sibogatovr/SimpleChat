using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace SimpleChat.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        var seqProperties = builder.Configuration.GetSection("Seq");
        var seqUrl = seqProperties["ServerUrl"] ?? throw new InvalidOperationException("ServerUrl config value empty");
        
        builder.Host.UseSerilog((context, provider, loggerConfiguration) =>
        {
            var controlLevelSwitch = new LoggingLevelSwitch();
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(provider)
                .Enrich.WithProperty("Application", typeof(Program).Assembly.GetName().Name)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.log", rollingInterval: RollingInterval.Day)
                .WriteTo.File(new CompactJsonFormatter(), "Logs/audit-.json", rollingInterval: RollingInterval.Day)
                .WriteTo.Seq(seqUrl, apiKey: seqProperties["ApiKey"], controlLevelSwitch: controlLevelSwitch)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Infrastructure", LogEventLevel.Warning)
                .MinimumLevel.Override("System.Data", LogEventLevel.Warning);
        });
        
        LogHost.ServiceProvider = builder.Services.BuildServiceProvider();
        
        return builder;
    }
    
}