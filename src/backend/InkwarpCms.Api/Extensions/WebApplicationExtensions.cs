using InkwarpCms.Api.Middlewares;
using Serilog;

namespace InkwarpCms.Api.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder RegisterMiddlewares(this IApplicationBuilder app)
    {
        app.UseGlobalExceptionHandlingMiddleware();
        return app;
    }

    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            app.UseSerilogRequestLogging();
        }
        app.RegisterMiddlewares();
        return app;
    }
}
