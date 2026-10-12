using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

using OnlineStore.Functions.DependencyInjection;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddFunctionServices();

builder.Build().Run();
