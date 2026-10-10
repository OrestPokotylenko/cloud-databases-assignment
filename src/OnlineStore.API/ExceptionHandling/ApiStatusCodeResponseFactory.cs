using System.Diagnostics;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;

namespace OnlineStore.API.ExceptionHandling;

public static class ApiStatusCodeResponseFactory
{
    public static Task CreateAsync(StatusCodeContext context)
    {
        var httpContext = context.HttpContext;
        var status = httpContext.Response.StatusCode;
        var title = ReasonPhrases.GetReasonPhrase(status);

        var response = new ApiErrorResponse(
            string.IsNullOrWhiteSpace(title) ? "Request failed" : title,
            GetError(status),
            Activity.Current?.Id ?? httpContext.TraceIdentifier);

        return httpContext.Response.WriteAsJsonAsync(response);
    }

    private static string GetError(int status) => status switch
    {
        StatusCodes.Status400BadRequest => "The request is invalid.",
        StatusCodes.Status401Unauthorized => "Authentication is required.",
        StatusCodes.Status403Forbidden => "Access to this resource is forbidden.",
        StatusCodes.Status404NotFound => "The requested resource was not found.",
        StatusCodes.Status405MethodNotAllowed => "The HTTP method is not allowed for this resource.",
        StatusCodes.Status406NotAcceptable => "The requested response format is not supported.",
        StatusCodes.Status409Conflict => "The request conflicts with the current resource state.",
        StatusCodes.Status415UnsupportedMediaType => "The request content type is not supported.",
        StatusCodes.Status429TooManyRequests => "Too many requests were sent.",
        >= StatusCodes.Status500InternalServerError => "An unexpected error occurred.",
        _ => $"The request failed with HTTP status {status}."
    };
}
