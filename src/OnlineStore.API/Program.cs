using Asp.Versioning;

using OnlineStore.API.ExceptionHandling;
using OnlineStore.Application;
using OnlineStore.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory =
            InvalidModelStateResponseFactory.Create;
        options.SuppressMapClientErrors = true;
    });

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

builder.Services.AddApiVersioning(options =>
    {
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
        options.ReportApiVersions = true;
    })
    .AddMvc();

builder.Services.AddApplication();
builder.Services.AddPersistence(
    builder.Configuration.GetConnectionString("OnlineStore")
    ?? throw new InvalidOperationException(
        "Connection string 'OnlineStore' is not configured."));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseStatusCodePages(ApiStatusCodeResponseFactory.CreateAsync);
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
