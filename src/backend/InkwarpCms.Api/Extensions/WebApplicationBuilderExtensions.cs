using Serilog;

namespace InkwarpCms.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureApplicationBuilder(
        this WebApplicationBuilder builder
    )
    {
        builder
            .Host
            .UseSerilog(
                (context, configuration) =>
                    configuration.ReadFrom.Configuration(context.Configuration)
            );
        return builder;
    }
}
