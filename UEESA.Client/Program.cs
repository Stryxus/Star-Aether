using System.Net.WebSockets;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

using UEESA.Sockets;
using UEESA.Client;
using UEESA.Client.Data;
using UEESA.Client.Data.States;
using UEESA.Client.Sockets;
using UEESA.Client.Sockets.Handlers;

WebAssemblyHost Host;

WebAssemblyHostBuilder HostBuilder = WebAssemblyHostBuilder.CreateDefault(args);
Services.SetConfiguration(HostBuilder.Configuration);
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

HostBuilder.Services.AddHttpClient("UEESA.Server.ServerAPI", client => client.BaseAddress = new Uri(HostBuilder.HostEnvironment.BaseAddress)).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
HostBuilder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("UEESA.Server.ServerAPI"));

HostBuilder.Services.AddMsalAuthentication(options =>
{
    HostBuilder.Configuration.Bind("AZURE_AD_B2C", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://staraether.onmicrosoft.com/10bf1403-384e-47cd-80cc-f1caca6527b4/AAD.B2C.API.Access");
});

Host = HostBuilder.Build();
Services.SetServiceProvider(Host.Services);
await Host.RunAsync();