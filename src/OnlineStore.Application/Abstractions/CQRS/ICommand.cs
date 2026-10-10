namespace OnlineStore.Application.Abstractions.CQRS;

public interface ICommand<TResponse> : IRequest<TResponse>
{
}
