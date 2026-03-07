namespace Web.Infrastructure;

public class ErrorResponse
{
    public bool Success { get; init; } = false;
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public object? Errors { get; init; }
    public string TraceId { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}