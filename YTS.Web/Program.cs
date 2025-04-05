using Microsoft.Extensions.AI;
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

app.Run();

public class ServiceKeys
{
    public const string Mistral = "mistral";
}