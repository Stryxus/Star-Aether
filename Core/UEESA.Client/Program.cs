using System.Net.WebSockets;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using UEESA.Sockets;
using UEESA.Client;
using UEESA.Client.Data;
using UEESA.Client.Data.States;
using UEESA.Client.Sockets;
using UEESA.Client.Sockets.Handlers;

WebAssemblyHost Host;

WebAssemblyHostBuilder HostBuilder = WebAssemblyHostBuilder.CreateDefault(args);
HostBuilder.RootComponents.Add<App>("#app");
HostBuilder.Services.AddTransient<ConnectionManager<ClientWebSocket>>();
HostBuilder.Services.AddScoped<StateSocketHandler>();
HostBuilder.Services.AddScoped<ClientState>();
HostBuilder.Services.AddScoped<InitializationState>();
HostBuilder.Services.AddScoped<ServerState>();
HostBuilder.Services.AddScoped<LocalStorageState>();
HostBuilder.Services.AddScoped<WebSocketManagerMiddleware>();

HostBuilder.Services.AddScoped<UIState>();
HostBuilder.Services.AddScoped<UIState.PageState>();
HostBuilder.Services.AddScoped<UIState.ComponentState>();

HostBuilder.Services.AddScoped<JSInterface>();
HostBuilder.Services.AddScoped<JSInterface.Utilities>();
HostBuilder.Services.AddScoped<JSInterface.Runtime>();
HostBuilder.Services.AddScoped<JSInterface.Cache>();
HostBuilder.Services.AddScoped<JSInterface.LocalData>();
HostBuilder.Services.AddScoped<JSInterface.AnimationManager>();

Host = HostBuilder.Build();
Services.SetServiceProvider(Host.Services);
await Host.RunAsync();