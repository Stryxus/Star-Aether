using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;

using SA.Web.Server.WebSockets;
using SA.Web.Shared;
using SA.Web.Server.Data;
using SA.Web.Server.Models;

using SA.Web.Server.Data.Json;
using SA.Web.Server.Data.Identity;

using Shyjus.BrowserDetection;

namespace SA.Web.Server
{
    public class Program
    {
        internal static IConfiguration Configuration { get; private set; }

        public static async Task Main() => await Host.CreateDefaultBuilder().ConfigureWebHostDefaults((webBuilder) => 
            {
                webBuilder.ConfigureServices((services) =>
                {
                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
                    services.AddDatabaseDeveloperPageExceptionFilter();
                    services.AddDefaultIdentity<UEESACitizen>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
                    services.AddIdentityServer().AddApiAuthorization<UEESACitizen, ApplicationDbContext>();
                    services.AddAuthentication().AddIdentityServerJwt();
                    services.AddControllersWithViews();
                    services.AddRazorPages();

                    services.AddApplicationInsightsTelemetry(PrivateVariables.Instance.ApplicationInsightsKey);
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
                    services.AddControllersWithViews();
                    services.AddRazorPages();
                    services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/Pages");
                    services.AddRouting();
                    services.AddResponseCompression(options => options.Providers.Add<BrotliCompressionProvider>());
                    services.AddWebSocketManager();
                });

                webBuilder.Configure(async (app) => 
                {
                    Configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
                    Services.SetServiceProvider(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
                    Globals.IsDevelopmentMode = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();

                    if (Globals.IsDevelopmentMode)
                    {
                        app.UseDeveloperExceptionPage();
                        app.UseMigrationsEndPoint();
                        app.UseWebAssemblyDebugging();
                        app.UseSwagger();
                        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", Globals.APINameString + " " + Globals.APIVersionString));
                    }
                    else
                    {
                        app.UseExceptionHandler("/Error");
                        app.UseHsts();
                    }

                    app.UseBlazorFrameworkFiles();
                    app.UseStaticFiles();
                    app.UseResponseCompression();
                    app.UseWebSockets();
                    app.MapWebSocketManager("/state", Services.Get<StateSocketHandler>());
                    app.UseRouting();
                    app.UseIdentityServer();
                    app.UseAuthentication();
                    app.UseAuthorization();
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
                });
            }).Build().RunAsync();
    }
}
