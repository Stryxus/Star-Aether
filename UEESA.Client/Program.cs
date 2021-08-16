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
HostBuilder.Services.AddSingleton<StateSocketHandler>();
HostBuilder.Services.AddSingleton<ClientState>();
HostBuilder.Services.AddSingleton<InitializationState>();
HostBuilder.Services.AddSingleton<ServerState>();
HostBuilder.Services.AddSingleton<LocalStorageState>();
HostBuilder.Services.AddSingleton<WebSocketManagerMiddleware>();
HostBuilder.Services.AddSingleton<UserState>();

HostBuilder.Services.AddSingleton<UIState>();
HostBuilder.Services.AddSingleton<UIState.PageState>();
HostBuilder.Services.AddSingleton<UIState.ComponentState>();

HostBuilder.Services.AddSingleton<JSInterface>();
HostBuilder.Services.AddSingleton<JSInterface.Utilities>();
HostBuilder.Services.AddSingleton<JSInterface.Runtime>();
HostBuilder.Services.AddSingleton<JSInterface.Cache>();
HostBuilder.Services.AddSingleton<JSInterface.LocalData>();
HostBuilder.Services.AddSingleton<JSInterface.AnimationManager>();

Host = HostBuilder.Build();
Services.SetServiceProvider(Host.Services);
await Host.RunAsync();