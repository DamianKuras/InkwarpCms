using System.Net;
using System.Net.Mime;
using InkwarpCms.Api.Middlewares;
using InkwarpCms.Api.UnitTests.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace InkwarpCms.Api.UnitTests.Middlewares;

public class GlobalExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldPassRequestToNextDelegate_Always()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var mockDelegate = new Mock<RequestDelegate>();
        mockDelegate.Setup(x => x(context)).Returns(Task.CompletedTask);
        var middleware = new GlobalExceptionHandlingMiddleware(
            mockDelegate.Object,
            Mock.Of<ILogger<GlobalExceptionHandlingMiddleware>>()
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        mockDelegate.Verify(x => x(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ShouldLogException_WhenExceptionIsThrown()
    {
        // Arrange
        var ex = new GlobalExceptionHandlerTestException();
        var loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
        var middleware = new GlobalExceptionHandlingMiddleware(
            next: _ => throw ex,
            loggerMock.Object
        );
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        loggerMock.VerifyLog(
            logger => logger.LogCritical(ex, GlobalExceptionHandlingMiddleware.ExceptionLogMessage)
        );
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnJson_WhenExceptionIsThrown()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
        var middleware = new GlobalExceptionHandlingMiddleware(
            next: _ => throw new GlobalExceptionHandlerTestException(),
            loggerMock.Object
        );
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(MediaTypeNames.Application.Json, context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnStatusInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
        var middleware = new GlobalExceptionHandlingMiddleware(
            next: _ => throw new GlobalExceptionHandlerTestException(),
            loggerMock.Object
        );
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnCorrectSerializedProblemDetails_WhenExceptionIsThrown()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
        var middleware = new GlobalExceptionHandlingMiddleware(
            next: _ => throw new GlobalExceptionHandlerTestException(),
            loggerMock.Object
        );
        var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Equal(GlobalExceptionHandlingMiddleware.SerializedProblemDetails, responseBody);
    }
}
