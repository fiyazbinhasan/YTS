var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama")
    .WithDataVolume()
    .WithGPUSupport();

var mistral = ollama.AddModel("mistral", "mistral:7b");

builder.AddProject<Projects.YTS_McpServer>("mcpserver");

builder.AddProject<Projects.YTS_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(mistral)
    .WaitFor(mistral);

builder.Build().Run();
