using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace InkwarpCms.Api.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;
    private static readonly string InternalServerErrorDetail =
        "An internal server error has occurred";

    public static readonly string ExceptionLogMessage = "Unhandled exception occurred";

    private static readonly ProblemDetails DefaultProblemDetails =
        new()
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Type = nameof(HttpStatusCode.InternalServerError),
            Title = nameof(HttpStatusCode.InternalServerError),
            Detail = InternalServerErrorDetail
        };

    public static readonly string SerializedProblemDetails = JsonSerializer.Serialize(
        DefaultProblemDetails
    );

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ExceptionLogMessage);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(SerializedProblemDetails);
        }
    }
}

public static class GlobalExceptionHandlingMiddlewareExtension
{
    public static IApplicationBuilder UseGlobalExceptionHandlingMiddleware(
        this IApplicationBuilder app
    )
    {
        return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
