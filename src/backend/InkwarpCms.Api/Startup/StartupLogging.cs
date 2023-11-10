using Serilog;
using Serilog.Formatting.Compact;

namespace InkwarpCms.Api.Startup;

public static class StartupLogging
{
    /// <summary>
    ///     Configures logging before the creation of the ASP.NET Core host to catch
    ///     exceptions thrown during setup.
    ///     This configuration can be replaced later.
    /// </summary>
    public static void CreateBootstrapLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich
            .FromLogContext()
            .WriteTo
            .Console(new RenderedCompactJsonFormatter())
            .CreateBootstrapLogger();
    }
}
