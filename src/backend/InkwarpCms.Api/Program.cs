using InkwarpCms.Api.Extensions;
using Serilog;

namespace InkwarpCms.Api;

public static class Program
{
    public static async Task Main(string[] args)
    {
        StartupLogging.ConfigureBootstrapLogger();
        try
        {
            Log.Information("Starting web application");
            var builder = WebApplication.CreateBuilder(args).ConfigureApplicationBuilder();
            var app = builder.Build().ConfigureApplication();
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
