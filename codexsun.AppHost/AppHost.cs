var builder = DistributedApplication.CreateBuilder(args);

var server = builder.AddProject<Projects.cxserver>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

var webfrontend = builder.AddViteApp("webfrontend", "../cxweb")
    .WithEnvironment("PORT", "7043")
    .WithEndpoint("http", endpoint =>
    {
        endpoint.Port = 7043;
        endpoint.IsProxied = false;
    }, createIfNotExists: false)
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
