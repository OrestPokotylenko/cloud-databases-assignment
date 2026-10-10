# Online Store cloud databases assignment

The product feature is an ASP.NET Core Web API backed by SQL Server through
Entity Framework Core. It follows the solution's Domain, Application,
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

## Local setup

Store the SQL Server connection string in ASP.NET Core User Secrets. The API
project already has a `UserSecretsId`, so run this once from the repository
root:

```powershell
dotnet user-secrets set "ConnectionStrings:OnlineStore" `
  "Server=.\SQLEXPRESS;Database=OnlineStore;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True" `
  --project src/OnlineStore.API
```

User Secrets are stored outside the repository and must not be committed. In a
deployed environment, provide the same setting through the platform's secure
configuration using the key `ConnectionStrings__OnlineStore`.

```powershell
dotnet ef database update `
  --project src/OnlineStore.Persistence `
  --startup-project src/OnlineStore.API `
  -- --environment Development

dotnet run --project src/OnlineStore.API
dotnet test
```

Example requests are in `src/OnlineStore.API/OnlineStore.API.http`.

Product specifications are stored in SQL Server. Product image binaries are
deliberately outside this slice; they should be implemented as a separate blob
storage feature with only their metadata/reference associated with a product.
