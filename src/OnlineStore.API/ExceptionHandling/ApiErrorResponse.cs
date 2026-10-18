namespace OnlineStore.API.ExceptionHandling;

public record ApiErrorResponse(
    string Title,
    string Errors,
    string TraceId);
