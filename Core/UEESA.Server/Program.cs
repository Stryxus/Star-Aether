using System;
using System.Security.Authentication;

using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

using UEESA.Server.Sockets;
using UEESA.Server.Sockets.Handlers;
using UEESA.Server.Data;

IConfiguration Configuration;
string CORSAuthorityName = "_starAetherCORSAuthority";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(kestrelOptions =>
{
    kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.SslProtocols = SslProtocols.Tls13;
    });
});

#if DEBUG
builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { ConnectionString = PrivateData.Instance.DEV_ApplicationInsightsConnectionString });
if (!PrivateData.Instance.DEV_MicrosoftIdentityPlatformClientID.IsEmpty())
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddMicrosoftIdentityWebApp(options =>
    {
        options.Instance = "https://login.microsoftonline.com/";
        options.ClientId = PrivateData.Instance.DEV_MicrosoftIdentityPlatformClientID;
        options.TenantId = "common";
    });
}
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORSAuthorityName,
                      builder =>
                      {
                          builder.WithOrigins("https://localhost:5001");
                      });
});
#else
                    services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { ConnectionString = PrivateData.Instance.ApplicationInsightsConnectionString });
                    services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    }).AddMicrosoftIdentityWebApp(options =>
                    {
                        options.Instance = "https://login.microsoftonline.com/";
                        options.ClientId = PrivateData.Instance.MicrosoftIdentityPlatformClientID;
                        options.TenantId = "common";
                    });
                    services.AddCors(options =>
                    {
                        options.AddPolicy(name: "_starAetherCORSAuthority",
                                          builder =>
                                          {
                                              builder.WithOrigins("https://staraether.com");
                                          });
                    });
#endif

builder.Services.AddControllersWithViews(options =>
{
    AuthorizationPolicy policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddRazorPages().AddMicrosoftIdentityUI();
builder.Services.AddRouting();

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddResponseCaching();
builder.Services.AddWebSocketManager();
builder.Services.AddSingleton<MongoDBHandler>();
builder.Services.AddSingleton<RSIRoadmapScraper>();

WebApplication app = builder.Build();

Configuration = app.Configuration;
Services.SetServiceProvider(app.Services.CreateScope().ServiceProvider);
Globals.IsDevelopmentMode = app.Environment.IsDevelopment();

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
app.UseWebSockets();
app.MapWebSocketManager("/state", Services.Get<StateSocketHandler>());
app.UseRouting();
app.UseCors(CORSAuthorityName);
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
    endpoints.MapFallbackToFile("index.html");
});

Services.Get<MongoDBHandler>();
Services.Get<RSIRoadmapScraper>();

await app.RunAsync();