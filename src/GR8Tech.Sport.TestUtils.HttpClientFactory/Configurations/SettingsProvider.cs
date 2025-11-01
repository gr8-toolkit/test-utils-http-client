using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace GR8Tech.Sport.TestUtils.HttpClientFactory.Configurations;

internal static class SettingsProvider
{
    internal static IConfiguration Configuration { get; }
    internal static ILogger Logger { get; }
    
    static SettingsProvider()
    {
        var configFileName = File.Exists("test-settings.json") ? "test-settings" : "appsettings";
        
        Configuration = new ConfigurationBuilder()
            .AddJsonFile($"{configFileName}.json", false, true)
            .AddJsonFile($"{configFileName}.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
            .AddEnvironmentVariables()
            .Build();

        Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .WriteTo.Async(x => x.Console(theme:AnsiConsoleTheme.Code), 10)
            .CreateLogger();

        Logger.ForContext("SourceContext", typeof(SettingsProvider))
            .Information("HttpClientFactorySettings have been read and initialized");
    }
}
