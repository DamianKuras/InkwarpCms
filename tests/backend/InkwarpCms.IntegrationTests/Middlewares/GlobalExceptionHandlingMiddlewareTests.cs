using System.Net;
using System.Net.Mime;
using InkwarpCms.Api.Extensions;
using InkwarpCms.Api.Middlewares;
using InkwarpCms.IntegrationTests.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace InkwarpCms.IntegrationTests.Middlewares;


public class GlobalExceptionHandlingMiddlewareTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly string _exceptionEndpointPath = "/exception";

    private readonly string _testEndpointPath = "/test";

    public GlobalExceptionHandlingMiddlewareTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private IHostBuilder CreateTestServerWithExceptionThrowingEndpoint()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureTestLogging(_testOutputHelper)
                    .Configure(app =>
                    {
                        app.RegisterMiddlewares();
                        app.Map(
                            _exceptionEndpointPath,
                            appBuilder =>
                                appBuilder.Run(_ => throw new GlobalExceptionHandlerTestException())
                        );
                    });
            });
    }

    private IHostBuilder CreateTestServerWithExceptionThrowingMiddleware()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureTestLogging(_testOutputHelper)
                    .Configure(app =>
                    {
                        app.RegisterMiddlewares();
                        app.Use(
                            (HttpContext _, Func<Task> _) =>
                                throw new GlobalExceptionHandlerTestException()
                        );
                        app.Map(
                            _testEndpointPath,
                            appBuilder => appBuilder.Run(_ => Task.FromResult("Ok"))
                        );
                    });
            });
    }

    [Fact]
    public async Task ExceptionEndpoint_ShouldReturnInternalServerErrorWithProblemDetails_WhenUnhandledExceptionOccursInEndpoint()
    {
        // Arrange
        using var host = await CreateTestServerWithExceptionThrowingEndpoint().StartAsync();
        using var client = host.GetTestClient();
        // Act
        var response = await client.GetAsync(_exceptionEndpointPath);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal(
            MediaTypeNames.Application.Json,
            response.Content.Headers.ContentType.MediaType
        );
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseBody);
        Assert.Equal(GlobalExceptionHandlingMiddleware.SerializedProblemDetails, responseBody);
    }

    [Fact]
    public async Task TestEndpoint_ShouldReturnInternalServerErrorWithProblemDetails_WhenUnhandledExceptionOccursInMiddleware()
    {
        // Arrange
        using var host = await CreateTestServerWithExceptionThrowingMiddleware().StartAsync();
        using var client = host.GetTestClient();

        // Act
        var response = await client.GetAsync(_testEndpointPath);

        //Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal(
            MediaTypeNames.Application.Json,
            response.Content.Headers.ContentType.MediaType
        );
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseBody);
        Assert.Equal(GlobalExceptionHandlingMiddleware.SerializedProblemDetails, responseBody);
    }
}
