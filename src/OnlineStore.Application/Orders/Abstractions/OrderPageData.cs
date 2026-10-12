namespace OnlineStore.Application.Orders.Abstractions;

public record OrderPageData(
    IReadOnlyList<OrderData> Orders,
    int TotalCount);
