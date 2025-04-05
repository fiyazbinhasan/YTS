using Microsoft.Extensions.AI;
using OllamaSharp.ModelContextProtocol;
using OllamaSharp.ModelContextProtocol.Server;
using System.Text.Json;
using System.Text.Json.Serialization;
using YTS.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder
    .AddKeyedOllamaApiClient(ServiceKeys.Mistral)
    .AddKeyedChatClient()
    .UseFunctionInvocation()
    .UseOpenTelemetry();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/ollama", async ([FromKeyedServices(ServiceKeys.Mistral)] IChatClient client, string prompt) =>
{
    var serverConfig = new McpServerConfiguration
    {
        Name = "Ollama Math MCP Server",
        Command = "http://localhost:5062/sse",
        TransportType = McpServerTransportType.Sse
    };

    var mcpTools = await Tools.GetFromMcpServers(serverConfig);

    var tools = mcpTools.Cast<McpClientTool>()
        .Select(AITool (t) => new McpFunctionAdapter(t))
        .ToList();

    var response = await client.GetResponseAsync(prompt, new ChatOptions
    {
        Tools = [.. tools]
    });

    return response.Text;
});

app.Run();

public class ServiceKeys
{
    public const string Mistral = "mistral";
}

public class McpFunctionAdapter(McpClientTool mcpTool) : AIFunction
{
    private readonly McpClientTool _mcpTool = mcpTool ?? throw new ArgumentNullException(nameof(mcpTool));

    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public override string Name => _mcpTool.Function?.Name ?? "Unknown Function";

    public override string Description => _mcpTool.Function?.Description ?? "No description available";

    public override IReadOnlyDictionary<string, object?> AdditionalProperties =>
        _mcpTool.Function?.Parameters?.Properties?.ToDictionary(
            kvp => kvp.Key, object? (kvp) => new { kvp.Value.Type, kvp.Value.Description }
        ) ?? new Dictionary<string, object?>();

    public override JsonElement JsonSchema => GetJsonSchema();

    public override JsonSerializerOptions JsonSerializerOptions => DefaultJsonSerializerOptions;

    protected override async Task<object?> InvokeCoreAsync(IEnumerable<KeyValuePair<string, object?>> arguments,
        CancellationToken cancellationToken)
    {
        var argsDict = arguments.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return await _mcpTool.InvokeMethodAsync(argsDict);
    }

    private JsonElement GetJsonSchema()
    {
        if (_mcpTool.Function?.Parameters == null)
        {
            return JsonDocument.Parse("{}").RootElement;
        }

        var schema = new
        {
            type = _mcpTool.Function.Parameters.Type,
            properties = _mcpTool.Function.Parameters.Properties?.ToDictionary(
                kvp => kvp.Key, object? (kvp) => new { type = kvp.Value.Type, description = kvp.Value.Description }
            ) ?? new Dictionary<string, object?>(),
            required = _mcpTool.Function.Parameters.Required
        };

        var json = JsonSerializer.Serialize(schema, JsonSerializerOptions);
        return JsonDocument.Parse(json).RootElement;
    }
}