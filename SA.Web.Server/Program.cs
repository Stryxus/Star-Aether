using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using SA.Web.Server.WebSockets;
using SA.Web.Shared;
using SA.Web.Server.Data;

using SA.Web.Server.Data.Json;
using SA.Web.Server.Discord;

using Shyjus.BrowserDetection;
using Microsoft.AspNetCore.Http;

namespace SA.Web.Server
{
    public class Program
    {
        public static async Task Main() => await Host.CreateDefaultBuilder().ConfigureWebHostDefaults((webBuilder) => 
            {
#if RELEASE
                webBuilder.ConfigureServices(async (services) =>
                {
                    await Logger.LogInfo("Starting Application Insights Telemetry");
                    services.AddApplicationInsightsTelemetry(PrivateVariables.Instance.ApplicationInsightsKey);
                    services.AddApplicationInsightsTelemetryProcessor<InsightsFilter>();
                    await Logger.LogInfo("Started Application Insights Telemetry");
#else
                webBuilder.ConfigureServices((services) => 
                {
#endif
                    services.AddBrowserDetection();
                    services.AddScoped<MongoDBInterface>();
                    services.AddScoped<TwitchInterface>();
                    services.AddControllers();
                    services.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc(Globals.APIVersionString,
                            new OpenApiInfo
                            {
                                Title = Globals.APINameString,
                                Version = Globals.APIVersionString,
                            });
                    });
                    services.AddSwaggerGenNewtonsoftSupport();
                    services.AddRazorPages();
                    services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/Pages");
                    services.AddRouting();
                    services.AddResponseCompression(options => options.Providers.Add<BrotliCompressionProvider>());
                    services.AddWebSocketManager();
                });

                webBuilder.Configure(async (app) => 
                {
                    Services.SetServiceProvider(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
                    Globals.IsDevelopmentMode = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();

                    if (Globals.IsDevelopmentMode)
                    {
                        app.UseDeveloperExceptionPage();
                        app.UseWebAssemblyDebugging();
                        app.UseSwagger();
                        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", Globals.APINameString + " " + Globals.APIVersionString));
                    }
                    else app.UseExceptionHandler("/Error");

                    app.UseBlazorFrameworkFiles();
                    app.UseStaticFiles();
                    app.UseResponseCompression();
                    app.UseWebSockets();
                    app.MapWebSocketManager("/state", Services.Get<StateSocketHandler>());
                    app.UseRouting();
                    app.Use(async (context, next) =>
                    {
                        bool isAllowed = false;
                        try
                        {
                            isAllowed = Services.Get<IBrowserDetector>().Browser.Name.Contains("Chrome") || Services.Get<IBrowserDetector>().Browser.Name.Contains("Chromium");
                        }
                        catch { isAllowed = true; }
                        if (!isAllowed) context.Response.Redirect("chrome.html");
                        else await next();
                    });
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapRazorPages();
                        endpoints.MapControllers();
                        endpoints.MapFallbackToPage("/_Host");
                    });
                    await Services.Get<MongoDBInterface>().Connect();
                    Services.Get<TwitchInterface>().Connect();
                    if (ServerLocator.IsUKServer()) SABotDiscord.StartBot(Globals.APIVersionString, 
                                                                            Globals.IsDevelopmentMode ? PrivateVariables.Instance.DebugToken : PrivateVariables.Instance.Token, 
                                                                            PrivateVariables.Instance.GuildID, 
                                                                            PrivateVariables.Instance.DebugChannelID);
                });
            }).Build().RunAsync();
    }
}
