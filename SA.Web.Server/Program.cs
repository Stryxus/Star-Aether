using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using SA.Web.Server.WebSockets;
using SA.Web.Shared;

namespace SA.Web.Server
{
    public class Program
    {
        internal static IConfiguration Configuration { get; private set; }

        public static async Task Main() => await Host.CreateDefaultBuilder().ConfigureWebHostDefaults((webBuilder) => 
            {
                webBuilder.ConfigureServices((services) =>
                {
                    services.AddRazorPages();
                    services.AddScoped<MongoDBInterface>();
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
                    app.UseWebSockets();
                    app.MapWebSocketManager("/state", Services.Get<StateSocketHandler>());
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapRazorPages();
                        endpoints.MapFallbackToPage("/_Host");
                    });
                    await Services.Get<MongoDBInterface>().Connect();
                });
            }).Build().RunAsync();
    }
}
