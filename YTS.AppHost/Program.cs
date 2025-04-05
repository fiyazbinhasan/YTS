var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama")
    .WithDataVolume()
    .WithGPUSupport()
    .WithOpenWebUI();

var deepseek = ollama.AddModel("deepseek", "deepseek-r1:8b");
var llama = ollama.AddHuggingFaceModel("llama", "bartowski/Llama-3.2-1B-Instruct-GGUF:IQ4_XS");

builder.AddProject<Projects.YTS_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(deepseek)
    .WaitFor(deepseek)
    .WithReference(llama)
    .WaitFor(llama);

builder.AddYarnApp("nextfrontend", "../yts-nextjs-app", "dev")
    .WithYarnPackageInstallation()
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.AddViteApp("sveltefrontend", "../yts-svelte-app", "pnpm")
    .WithPnpmPackageInstallation();

builder.AddBunApp("nestjsfrontend", "../yts-nestjs-bun-app", "start")
    .WithHttpEndpoint(env: "PORT")
    .WithBunPackageInstallation();

builder.Build().Run();
