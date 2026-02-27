using cxserver;
using cxserver.Application.DependencyInjection;
using cxserver.Endpoints;
using cxserver.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseTenantResolution();

app.MapConfigurationDocumentsEndpoints();
app.MapTenantsEndpoints();
app.MapDefaultEndpoints();
app.UseFileServer();

app.Run();
