namespace OnlineStore.Application.Abstractions.CQRS;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}
