using InkwarpCms.Api.Middlewares;
using Serilog;

namespace InkwarpCms.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}
