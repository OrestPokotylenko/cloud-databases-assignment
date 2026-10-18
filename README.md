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
- `POST /api/v1/products/{publicId}/images/upload`
- `POST /api/v1/products/{publicId}/images/{imagePublicId}/confirm`
- `DELETE /api/v1/products/{publicId}/images/{imagePublicId}`
- `POST /api/v1/products/{publicId}/images/{imagePublicId}/primary`
- `POST /api/v1/products/{publicId}/reviews`
- `GET /api/v1/products/{publicId}/reviews?pageSize=20&continuationToken=...`
- `DELETE /api/v1/products/{publicId}/reviews/{reviewPublicId}`

Page sizes are limited to 100 records. API consumers use the product's
`publicId`; the SQL identity key remains an implementation detail.

Product image metadata is stored in SQL Server while the image bytes are stored
in the public-read `product-images` Blob container. Uploads do not pass through
the API. The API returns a short-lived, write-only SAS URL for one blob; the
client uploads to that URL and then confirms the upload with the API. Public
image URLs returned by the API point directly to Blob Storage.

Anonymous product reviews are stored in the `productreviews` Azure Table. Azure
Table names only permit alphanumeric characters, so the intended
`product-reviews` name cannot be used. A product public ID is the partition key
and a review public ID is the row key. This makes retrieving reviews for one
product efficient and supports native continuation-token pagination without
SQL-style offsets. The design intentionally does not optimize cross-product
analytics. If global low-rating or marketing analysis is required later, review
data should be exported through a separate analytical pipeline or store.

## Order endpoints

- `GET /api/v1/orders?pageNumber=1&pageSize=20`
- `GET /api/v1/orders/{publicId}`
- `POST /api/v1/orders`
- `POST /api/v1/orders/{publicId}/process`
- `POST /api/v1/orders/{publicId}/ship`
- `POST /api/v1/orders/{publicId}/cancel`

Order creation and explicit order actions return `202 Accepted`. They are published to
Azure Storage Queues and processed by `CreateOrderFunction` and
`UpdateOrderFunction`. Creation verifies and decrements product stock in
the same SQL transaction as the order insert. Cancelling an unshipped order
restores its stock atomically. The domain entity permits only pending orders to
be processed, processing orders to be shipped, and pending or processing orders
to be cancelled. Shipped and cancelled orders are terminal.

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

dotnet user-secrets set "ProductImages:ConnectionString" `
  "UseDevelopmentStorage=true" `
  --project src/OnlineStore.API

dotnet user-secrets set "ProductReviews:ConnectionString" `
  "UseDevelopmentStorage=true" `
  --project src/OnlineStore.API
```

User Secrets are stored outside the repository and must not be committed. In a
deployed API environment, provide the settings through the platform's secure
configuration using `Database__ConnectionString` and
`Queues__ConnectionString`. The image feature uses
`ProductImages__ConnectionString`, and reviews use
`ProductReviews__ConnectionString`. Queue names are non-secret options in
`appsettings.json`; override them with `Queues__CreateOrderQueueName` and
`Queues__OrderUpdateQueueName` when needed. The local Functions host also needs
`AzureWebJobsStorage` and `FUNCTIONS_WORKER_RUNTIME`; the deployed Flex
Consumption runtime is configured through `functionAppConfig` instead.

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
    "Queues:OrderUpdateQueueName": "order-update",
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
dotnet run --project tests/OnlineStore.Application.UnitTests
```

Example requests are in `src/OnlineStore.API/OnlineStore.API.http`.

## Continuous integration

Pull requests targeting `develop` or `main` run the modular validation workflow
in `.github/workflows/_validation.yml`. It contains independent jobs for:

- restoring and building the .NET solution and API container;
- running the xUnit v3 tests;
- auditing direct and transitive NuGet dependencies and running CodeQL;
- linting and compiling the Bicep template and both parameter files.

The deployment workflow calls the same reusable validation workflow before it
can deploy. A push to `develop` deploys `dev`, while a push to `main` deploys
`prod`. Either environment can also be selected manually through
`workflow_dispatch`.

## Azure infrastructure

The subscription-scoped entry point is `infra/main.bicep`. Its modules deploy:

- an Azure Container Apps consumption environment and the API Container App;
- an Azure Container Registry used for the API image;
- one Flex Consumption Function App containing `CreateOrderFunction` and
  `UpdateOrderFunction`;
- an LRS StorageV2 account with the two order queues, Functions deployment
  container, public-read `product-images` Blob container, and `productreviews`
  Table Storage table;
- an Azure SQL logical server and serverless General Purpose database;
- Microsoft Entra-only SQL authentication;
- managed identities and least-scope pull, deployment, and database permissions;
- a Container Apps environment with live log streaming and no persisted log workspace.

Azure resources use the `<type>-onlinestore-<environment>` naming convention.
Globally scoped names add a deterministic five-character suffix, and resources
that don't allow hyphens omit them. Examples include `func-onlinestore-dev-abc12`,
`ca-onlinestore-dev`, `cae-onlinestore-dev`, `sql-onlinestore-dev-abc12`,
`sqldb-onlinestore-dev`, `id-onlinestore-api-dev`, and
`stonlinestoredevabc12`.

The `dev.bicepparam` and `prod.bicepparam` files contain separate
`containerAppSettings`, `functionAppSettings`, and `sqlSettings` objects. Their
values are currently identical:

- Container App: France Central, Consumption, 0.5 vCPU, 1 GiB, zero minimum
  replicas and 2 maximum replicas.
- Functions: France Central, Flex Consumption, 2,048 MB instances.
- SQL: General Purpose serverless, 0.5 minimum and 2 maximum vCores, 5 GiB,
  locally redundant backup storage, and a 30-minute auto-pause delay.

The deployment user-assigned managed identity is configured as the Azure SQL
Microsoft Entra administrator. The API and Functions use their own
user-assigned managed identities to connect to SQL, so there is no SQL username
or password to store. Bicep needs the deployment identity's name and client ID
when compiling the environment parameter files. To validate locally:

```powershell
$env:AZURE_CLIENT_ID = '<managed-identity-client-id>'
$env:AZURE_SQL_ADMINISTRATOR_NAME = '<managed-identity-name>'
az bicep lint --file infra/main.bicep
az bicep build --file infra/main.bicep
az bicep build-params --file infra/parameters/dev.bicepparam
```

You can retrieve the identity name and client ID after signing in to Azure:

```powershell
az identity show `
  --resource-group '<bootstrap-resource-group>' `
  --name '<managed-identity-name>' `
  --query '{name:name, clientId:clientId}' `
  --output json
```

Create protected GitHub environments named `dev` and `prod`, each with these
secrets:

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`

Add this non-secret environment variable to both GitHub environments:

- `AZURE_SQL_ADMINISTRATOR_NAME`

The Azure identity is authenticated through GitHub OIDC. It needs permission
to deploy resources and create the narrowly scoped managed-identity role
assignments; `Contributor` plus `User Access Administrator` at the target
subscription is sufficient for this assignment deployment.

The deployment workflow validates the real Azure deployment, provisions the
infrastructure, and applies EF Core migrations through a temporary SQL firewall
rule using the OIDC-authenticated Azure CLI identity. It then creates contained
database users for the API and Functions managed identities from their client
IDs, without requiring Microsoft Graph directory permissions. Finally, it
builds the API image in ACR, updates the Container App declaratively, and
publishes the Functions package.

The API container can also be built locally from the repository root:

```powershell
docker build -f src/OnlineStore.API/Dockerfile -t online-store-api .
```
