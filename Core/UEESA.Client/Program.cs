using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using UEESA.Client.Sockets;
using UEESA.Client.Sockets.Handlers;
using UEESA.Client.Data;
using UEESA.Client.Data.States;
using UEESA.Shared.Sockets;

namespace UEESA.Client
{
    public static class Program
    {
        public static WebAssemblyHost Host { get; private set; }

        public static async Task Main(string[] args)
        {
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
        }
    }
}
