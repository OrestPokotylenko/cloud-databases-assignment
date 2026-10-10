using Microsoft.Extensions.DependencyInjection;

namespace OnlineStore.Application.Abstractions.CQRS;

internal sealed class RequestSender(
    IServiceProvider serviceProvider)
    : IRequestSender
{
    public Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var handlerType = typeof(IHandler<,>).MakeGenericType(
            request.GetType(),
            typeof(TResponse));
        var handler = serviceProvider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod("HandleAsync")
            ?? throw new InvalidOperationException(
                $"Handler '{handlerType.Name}' does not expose HandleAsync.");

        return (Task<TResponse>)(handleMethod.Invoke(
            handler,
            [request, ct])
            ?? throw new InvalidOperationException(
                $"Handler '{handlerType.Name}' returned no task."));
    }
}
