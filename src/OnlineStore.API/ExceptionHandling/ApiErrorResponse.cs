namespace OnlineStore.API.ExceptionHandling;

public sealed record ApiErrorResponse(
    string Title,
    string Errors,
    string TraceId);
