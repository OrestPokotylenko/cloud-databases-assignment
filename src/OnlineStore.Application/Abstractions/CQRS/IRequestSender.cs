namespace OnlineStore.Application.Abstractions.CQRS;

public interface IRequestSender
{
    Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct = default);
}
