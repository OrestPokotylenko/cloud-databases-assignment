# Online Store cloud databases assignment

The product and order features are an ASP.NET Core Web API backed by SQL Server
through Entity Framework Core. They follow the solution's Domain, Application,
Persistence, and API layers. Commands and queries are handled separately and
are dispatched by the application request sender registered through dependency
injection.

## Product endpoints

- `GET /api/v1/products?pageNumber=1&pageSize=20`
- `GET /api/v1/products/{publicId}`
- `POST /api/v1/products`
- `PUT /api/v1/products/{publicId}`
- `DELETE /api/v1/products/{publicId}`

Page sizes are limited to 100 records. API consumers use the product's
`publicId`; the SQL identity key remains an implementation detail.

## Order endpoints

- `GET /api/v1/orders?pageNumber=1&pageSize=20`
- `GET /api/v1/orders/{publicId}`
- `POST /api/v1/orders`
- `PUT /api/v1/orders/{publicId}/status`

Order creation and status updates return `202 Accepted`. They are published to
Azure Storage Queues and processed by `CreateOrderFunction` and
`UpdateOrderStatusFunction`. Creation verifies and decrements product stock in
the same SQL transaction as the order insert. Cancelling an unshipped order
restores its stock atomically. Status values are case-insensitive.

Azure Storage Queues provide at-least-once delivery, so a message can be
delivered again. Each command therefore carries a `MessageId`, and the
`ProcessedMessages` SQL table is an infrastructure-only idempotency ledger.
The ledger entry and the order change are committed in the same transaction.

## Local setup

Store the SQL Server connection string in ASP.NET Core User Secrets. The API
project already has a `UserSecretsId`, so run this once from the repository
root:

```powershell
dotnet user-secrets set "Database:ConnectionString" `
  "Server=.\SQLEXPRESS;Database=OnlineStore;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True" `
  --project src/OnlineStore.API

dotnet user-secrets set "Queues:ConnectionString" `
  "UseDevelopmentStorage=true" `
  --project src/OnlineStore.API
```

User Secrets are stored outside the repository and must not be committed. In a
deployed API environment, provide the settings through the platform's secure
configuration using `Database__ConnectionString` and
`Queues__ConnectionString`. Queue names are non-secret options in
`appsettings.json`; override them with `Queues__CreateOrderQueueName` and
`Queues__UpdateOrderStatusQueueName` when needed. The Function App also needs
`AzureWebJobsStorage` and `FUNCTIONS_WORKER_RUNTIME`.

Install and start Azurite for local Azure Storage Queue development. The
Functions host needs these local settings through environment variables or an
ignored `local.settings.json`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "Queues:ConnectionString": "UseDevelopmentStorage=true",
    "Queues:CreateOrderQueueName": "order-create",
    "Queues:UpdateOrderStatusQueueName": "order-status-update",
    "Database:ConnectionString": "Server=.\\SQLEXPRESS;Database=OnlineStore;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

```powershell
dotnet ef database update `
  --project src/OnlineStore.Persistence `
  --startup-project src/OnlineStore.API `
  -- --environment Development

dotnet run --project src/OnlineStore.API
dotnet run --project src/OnlineStore.Functions
dotnet test
```

Example requests are in `src/OnlineStore.API/OnlineStore.API.http`.

Product specifications are stored in SQL Server. Product image binaries are
deliberately outside this slice; they should be implemented as a separate blob
storage feature with only their metadata/reference associated with a product.
