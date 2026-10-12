namespace OnlineStore.Application.Orders.Messages;

public record OrderCustomerMessage(
    string FullName,
    string Email,
    string? PhoneNumber);
