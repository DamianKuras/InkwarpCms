using InkwarpCms.Api.Extensions;
using InkwarpCms.Api.Startup;
using Serilog;

StartupLogging.CreateBootstrapLogger();
try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);
    _ = builder.ConfigureApplicationBuilder();
    var app = builder.Build();
    _ = app.ConfigureApplication();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
