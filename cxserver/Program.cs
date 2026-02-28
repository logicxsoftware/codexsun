using cxserver;
using cxserver.Application.DependencyInjection;
using cxserver.Endpoints;
using cxserver.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddSingleton(new ServerRuntimeInfo(DateTimeOffset.Now));
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseTenantResolution();

app.MapHomeEndpoints();
app.MapWebContentEndpoints();
app.MapMenuManagementEndpoints();
app.MapWebNavigationEndpoints();
app.MapSliderEndpoints();
app.MapConfigurationDocumentsEndpoints();
app.MapTenantContextEndpoints();
app.MapTenantsEndpoints();
app.MapProductCatalogEndpoints();
app.MapDefaultEndpoints();
app.UseFileServer();

app.Run();
