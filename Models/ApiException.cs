namespace TablexAPI.Models;

public class ApiException : Exception
{
    public int StatusCode { get; }

    public ApiException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : ApiException
{
    public NotFoundException(string message) : base(StatusCodes.Status404NotFound, message) { }
}

public class ConflictException : ApiException
{
    public ConflictException(string message) : base(StatusCodes.Status409Conflict, message) { }
}

public class BadRequestException : ApiException
{
    public BadRequestException(string message) : base(StatusCodes.Status400BadRequest, message) { }
}

public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message) : base(StatusCodes.Status401Unauthorized, message) { }
}

public class ForbiddenException : ApiException
{
    public ForbiddenException(string message) : base(StatusCodes.Status403Forbidden, message) { }
}
