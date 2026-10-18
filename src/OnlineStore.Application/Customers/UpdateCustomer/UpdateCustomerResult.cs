namespace OnlineStore.Application.Customers.UpdateCustomer;

public record UpdateCustomerResult(
    bool CustomerExists,
    bool EmailAvailable);
