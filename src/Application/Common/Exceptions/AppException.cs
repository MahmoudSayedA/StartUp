namespace Application.Common.Exceptions;
public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public IEnumerable<ErrorDetail>? Errors { get; }

    protected AppException(
        string message,
        int statusCode,
        IEnumerable<ErrorDetail>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}

public record ErrorDetail(string Field, string Message);


public class NotFoundException : AppException
{
    public NotFoundException(string resource, object id)
        : base($"{resource} with id '{id}' was not found.", 404) { }

    public NotFoundException(string message)
        : base(message, 404) { }
}

public class ValidationException : AppException
{
    public ValidationException(string message, IEnumerable<ErrorDetail>? errors = null)
    : base(message, 400, errors) { }

    public ValidationException(IEnumerable<ErrorDetail>? errors = null)
        : base("One or more validation errors occurred.", 400, errors) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized.")
        : base(message, 401) { }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "Access denied.")
        : base(message, 403) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message, 409) { }
}

public class BusinessRuleException : AppException
{
    public BusinessRuleException(string message, IEnumerable<ErrorDetail>? errors = null)
    : base(message, 422, errors) { }
}