using System.Diagnostics;

using Microsoft.AspNetCore.Diagnostics;

namespace OnlineStore.API.ExceptionHandling;

public class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken ct)
    {
        var isInvalidRequest = exception is ArgumentException;
        var status = isInvalidRequest
            ? StatusCodes.Status400BadRequest
            : StatusCodes.Status500InternalServerError;

        if (!isInvalidRequest)
        {
            logger.LogError(
                exception,
                "An unhandled exception occurred while processing the request.");
        }

        var response = new ApiErrorResponse(
            isInvalidRequest ? "Invalid request" : "Server error",
            isInvalidRequest
                ? exception.Message
                : "An unexpected error occurred.",
            Activity.Current?.Id ?? httpContext.TraceIdentifier);

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(response, ct);

        return true;
    }
}
