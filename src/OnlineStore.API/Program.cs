using OnlineStore.API.DependencyInjection;
using OnlineStore.API.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddApiServices();

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
