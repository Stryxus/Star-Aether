﻿using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
#if RELEASE
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
#endif

using UEESA.Server.WebSockets;
using UEESA.Server.Data;
using UEESA.Shared;

namespace UEESA.Server
{
    public class Program
    {
        internal static IConfiguration Configuration { get; private set; }

        public static async Task Main() => await Host.CreateDefaultBuilder().ConfigureWebHostDefaults((webBuilder) => 
            {
                webBuilder.ConfigureServices((services) =>
                {
#if DEBUG
                    services.AddApplicationInsightsTelemetry("00000000-0000-0000-0000-000000000000");
#else
                    services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { ConnectionString = PrivateData.Instance.ApplicationInsightsConnectionString });
#endif
                    services.AddRazorPages();
                    services.AddRouting();
                    services.AddResponseCompression(options =>
                    {
                        options.Providers.Add<BrotliCompressionProvider>();
                        options.Providers.Add<GzipCompressionProvider>();
                        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
                    });
                    services.AddResponseCaching();
                    services.AddWebSocketManager();
                    services.AddSingleton<RSIRoadmapScraper>();
                });

                webBuilder.Configure((app) => 
                {
                    Configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
                    Services.SetServiceProvider(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
                    Globals.IsDevelopmentMode = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();

                    if (Globals.IsDevelopmentMode)
                    {
                        app.UseDeveloperExceptionPage();
                        app.UseWebAssemblyDebugging();
                    }
                    else
                    {
                        app.UseExceptionHandler("/Error");
                        app.UseHsts();
                    }

                    app.UseBlazorFrameworkFiles();
                    app.UseStaticFiles();
                    app.UseResponseCompression();
                    app.UseResponseCaching();
                    app.UseWebSockets();
                    app.MapWebSocketManager("/state", Services.Get<StateSocketHandler>());
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapRazorPages();
                        endpoints.MapFallbackToFile("index.html");
                    });

                    Services.Get<RSIRoadmapScraper>();
                });
            }).Build().RunAsync();
    }
}
