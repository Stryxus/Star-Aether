﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Authentication;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
#if RELEASE
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
#endif

using UEESA.Server.Sockets;
using UEESA.Server.Sockets.Handlers;
using UEESA.Server.Data;

namespace UEESA.Server
{
    public class Program
    {
        internal static IConfiguration Configuration { get; private set; }

        public static async Task Main() => await Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder => 
            {
                webBuilder.UseKestrel(kestrelOptions =>
                {
                    kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
                    {
                        httpsOptions.SslProtocols = SslProtocols.Tls13;
                    });
                });

                webBuilder.ConfigureServices((services) =>
                {
#if DEBUG
                    services.AddApplicationInsightsTelemetry("00000000-0000-0000-0000-000000000000");
#else
                    services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { ConnectionString = PrivateData.Instance.ApplicationInsightsConnectionString });
#endif
                    if (!PrivateData.Instance.MicrosoftIdentityPlatformClientID.IsEmpty())
                    {
                        services.AddAuthentication(options =>
                        {
                            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        }).AddMicrosoftIdentityWebApp(options =>
                        {
                            options.Instance = "https://login.microsoftonline.com/";
                            options.ClientId = PrivateData.Instance.MicrosoftIdentityPlatformClientID;
                            options.TenantId = "common";
                        });
                    }
#if DEBUG
                    /*
                    services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    }).AddGoogle(options =>
                    {
                        options.ClientId = PrivateData.Instance.GoogleIdentityPlatformClientID;
                        options.ClientSecret = PrivateData.Instance.GoogleIdentityPlatformClientSecret;
                    });
                    */
#endif
                    services.AddControllersWithViews(options =>
                    {
                        AuthorizationPolicy policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                        options.Filters.Add(new AuthorizeFilter(policy));
                    });
                    services.AddRazorPages().AddMicrosoftIdentityUI();
                    services.AddRouting();
                    services.AddResponseCompression(options =>
                    {
                        options.Providers.Add<BrotliCompressionProvider>();
                        options.Providers.Add<GzipCompressionProvider>();
                        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
                    });
                    services.Configure<ForwardedHeadersOptions>(options =>
                    {
                        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
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
                        app.UseExceptionHandler("/error");
                        app.UseHsts();
                    }

                    app.UseBlazorFrameworkFiles();
                    app.UseHttpsRedirection();
                    app.UseStaticFiles();
                    app.UseResponseCompression();
                    app.UseResponseCaching();
                    app.UseWebSockets();
                    app.MapWebSocketManager("/state", Services.Get<StateSocketHandler>());
                    app.UseRouting();
                    app.UseAuthentication();
                    app.UseAuthorization();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                        endpoints.MapRazorPages();
                        endpoints.MapFallbackToFile("index.html");
                    });

                    Services.Get<RSIRoadmapScraper>();
                });
            }).Build().RunAsync();
    }
}
