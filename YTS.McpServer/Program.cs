using YTS.McpServer.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithTools<MathTools>();

var app = builder.Build();

app.MapMcp();

app.Run();
