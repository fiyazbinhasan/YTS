using YTS.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddKeyedOllamaSharpChatClient(ServiceKeys.Deepseek);
builder.AddKeyedOllamaSharpChatClient(ServiceKeys.Llama);

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
    public const string Deepseek = "deepseek";
    public const string Llama = "llama";
}