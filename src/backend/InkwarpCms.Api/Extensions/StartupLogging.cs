using Serilog;
using Serilog.Formatting.Compact;

namespace InkwarpCms.Api.Extensions;

public static class StartupLogging
{
    /// <summary>
    ///     Configures logging before the creation of the ASP.NET Core host
    ///     to catch exceptions thrown during setup.
    ///     This configuration will be replaced later with configuration from appsettings
    /// </summary>
    public static void ConfigureBootstrapLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich
            .FromLogContext()
            .WriteTo
            .Console(new RenderedCompactJsonFormatter())
            .CreateBootstrapLogger();
    }
}
