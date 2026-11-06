
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceOps.Application.Operations;
using ServiceOps.Infrastructure.Operations;
using ServiceOps.OperationsMcp;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IDeploymentReader, InMemoryDeploymentReader>();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<OperationsTools>();

await builder.Build().RunAsync();