using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InkwarpCms.IntegrationTests;

public static class ConfigurationTestLogging
{
    public static IWebHostBuilder ConfigureTestLogging(
        this IWebHostBuilder webBuilder,
        ITestOutputHelper testOutputHelper
    )
    {
        webBuilder.ConfigureLogging(builder =>
        {
            builder.ClearProviders();
            builder
                .Services
                .AddSingleton<ILoggerProvider>(
                    new XUnitLoggerProvider(testOutputHelper, appendScope: false)
                );
        });
        return webBuilder;
    }
}
