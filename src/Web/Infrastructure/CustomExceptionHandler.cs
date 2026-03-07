using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Web.Infrastructure;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;
    private readonly ILogger<CustomExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public CustomExceptionHandler(
        ILogger<CustomExceptionHandler> logger,
        IHostEnvironment env)
    {
        _logger = logger;
        _env = env;

        _exceptionHandlers = new()
        {
            { typeof(ValidationException),          HandleValidationException },
            { typeof(NotFoundException),            HandleNotFoundException },
            { typeof(UnauthorizedException),        HandleUnauthorizedException },
            { typeof(ForbiddenException),           HandleForbiddenException },
            { typeof(ConflictException),            HandleConflictException },
            { typeof(BusinessRuleException),        HandleBusinessRuleException },
            { typeof(UnauthorizedAccessException),  HandleUnauthorizedAccessException },
        };
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out var handler))
        {
            await handler.Invoke(httpContext, exception);
            return true;
        }

        // Unhandled → 500
        await HandleUnknownException(httpContext, exception);
        return true;
    }

    // ─────────────────────────────────────────
    // Handlers
    // ─────────────────────────────────────────

    private async Task HandleValidationException(HttpContext ctx, Exception ex)
    {
        var exception = (ValidationException)ex;
        var traceId = GetTraceId(ctx);

        _logger.LogWarning(
            "[{TraceId}] Validation failed: {Errors}",
            traceId,
            exception.Errors);

        ctx.Response.StatusCode = StatusCodes.Status400BadRequest;

        await ctx.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = exception.Message,
            Errors = exception.Errors,
            TraceId = traceId
        });
    }

    private async Task HandleNotFoundException(HttpContext ctx, Exception ex)
    {
        var traceId = GetTraceId(ctx);

        _logger.LogWarning(
            "[{TraceId}] Resource not found: {Message}",
            traceId,
            ex.Message);

        ctx.Response.StatusCode = StatusCodes.Status404NotFound;

        await ctx.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = StatusCodes.Status404NotFound,
            Message = ex.Message,
            TraceId = traceId
        });
    }

    private async Task HandleUnauthorizedException(HttpContext ctx, Exception ex)
    {
        var traceId = GetTraceId(ctx);

        _logger.LogWarning(
            "[{TraceId}] Unauthorized: {Message}",
            traceId,
            ex.Message);

        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await ctx.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = ex.Message,
            TraceId = traceId
        });
    }

    private async Task HandleForbiddenException(HttpContext ctx, Exception ex)
    {
        var traceId = GetTraceId(ctx);

        _logger.LogWarning(
            "[{TraceId}] Forbidden access: {Message}",
            traceId,
            ex.Message);

        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;

        await ctx.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = StatusCodes.Status403Forbidden,
            Message = ex.Message,
            TraceId = traceId
        });
    }

    private async Task HandleConflictException(HttpContext ctx, Exception ex)
    {
        var traceId = GetTraceId(ctx);

        _logger.LogWarning(
            "[{TraceId}] Conflict: {Message}",
            traceId,
            ex.Message);

        ctx.Response.StatusCode = StatusCodes.Status409Conflict;

        await ctx.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = StatusCodes.Status409Conflict,
            Message = ex.Message,
            TraceId = traceId
        });
    }

    private async Task HandleBusinessRuleException(HttpContext ctx, Exception ex)
    {
        var traceId = GetTraceId(ctx);

        _logger.LogWarning(
            "[{TraceId}] Business rule violation: {Message}",
            traceId,
            ex.Message);

        ctx.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

        await ctx.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity,
            Message = ex.Message,
            TraceId = traceId
        });
    }

    // .NET built-in — بتيجي من [Authorize] attribute
    private async Task HandleUnauthorizedAccessException(HttpContext ctx, Exception ex)
    {
        var traceId = GetTraceId(ctx);

        _logger.LogWarning(
            "[{TraceId}] Unauthorized access: {Message}",
            traceId,
            ex.Message);

        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await ctx.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = "Unauthorized.",
            TraceId = traceId
        });
    }

    // Catch-all — أي exception مش معروفة
    private async Task HandleUnknownException(HttpContext ctx, Exception ex)
    {
        var traceId = GetTraceId(ctx);

        // هنا Error مش Warning — دي حاجة مش متوقعة
        _logger.LogError(
            ex,
            "[{TraceId}] Unhandled exception: {Message}",
            traceId,
            ex.Message);

        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await ctx.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = _env.IsDevelopment()
                            ? ex.Message
                            : "An unexpected error occurred. Please try again later.",
            TraceId = traceId
        });
    }

    // ─────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────

    private static string GetTraceId(HttpContext ctx)
        => Activity.Current?.Id ?? ctx.TraceIdentifier;
}