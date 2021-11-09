using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

using UEESA.Client;
using UEESA.Client.Data;
using UEESA.Client.Data.States;
using UEESA.Client.Data.Authentication;

using Serilog;

Logger.Initialise(new LoggerConfiguration().WriteTo.Console(outputTemplate: Logger.DefaultLogFormat).CreateLogger());

WebAssemblyHost Host;
WebAssemblyHostBuilder HostBuilder = WebAssemblyHostBuilder.CreateDefault(args);
Services.SetConfiguration(HostBuilder.Configuration);
HostBuilder.RootComponents.Add<App>("#app");
HostBuilder.Services.AddSingleton<ClientState>(new ClientState());
HostBuilder.Services.AddSingleton<InitializationState>(new InitializationState());
HostBuilder.Services.AddSingleton<ServerState>(new ServerState());
HostBuilder.Services.AddSingleton<LocalStorageState>(new LocalStorageState());
HostBuilder.Services.AddSingleton<UserState>(new UserState());
HostBuilder.Services.AddSingleton<UIState>(new UIState());
HostBuilder.Services.AddSingleton<UIState.PageState>(new UIState.PageState());
HostBuilder.Services.AddSingleton<UIState.ComponentState>(new UIState.ComponentState());
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
HostBuilder.Services.AddAuthorizationCore(o =>
{
    o.AddPolicy("UserIsAdmin", policy => policy.Requirements.Add(new UserGroupsRequirement(new string[] { "Admins" })));
});
HostBuilder.Services.AddSingleton<IAuthorizationHandler, UserGroupsHandler>();
Host = HostBuilder.Build();
Services.SetServiceProvider(Host.Services);
await Host.RunAsync();
