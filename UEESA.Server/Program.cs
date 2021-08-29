using System.Security.Authentication;

using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

using UEESA;
using UEESA.Server.Data;
using UEESA.Server.Sockets;
using UEESA.Server.Sockets.Handlers;

string CORSAuthorityName = "_starAetherCORSAuthority";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
Services.SetConfiguration(builder.Configuration);

builder.WebHost.UseKestrel(kestrelOptions =>
{
    kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.SslProtocols = SslProtocols.Tls13;
    });
});

#if DEBUG
builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { ConnectionString = "00000000-0000-0000-0000-000000000000" });
if (Services.Configuration["DEV_MIP_CID"] != null && !Services.Configuration["DEV_MIP_CID"].IsEmpty())
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddMicrosoftIdentityWebApp(options =>
    {
        options.Instance = "https://login.microsoftonline.com/";
        options.ClientId = Services.Configuration["DEV_MIP_CID"];
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
builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { ConnectionString = Services.Configuration["APPINSIGHTS_CONNECTIONSTRING"] });
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddMicrosoftIdentityWebApp(options =>
    {
        options.Instance = "https://login.microsoftonline.com/";
        options.ClientId = Services.Configuration["MIP_CID"];
        options.TenantId = "common";
    });
builder.Services.AddCors(options =>
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

FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
provider.Mappings[".avif"] = "image/avif";

app.UseBlazorFrameworkFiles();
app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});
app.UseResponseCompression();
app.UseWebSockets();
app.MapWebSocketManager("/state", Services.Get<StateSocketHandler>());
app.UseRouting();
app.UseCors(CORSAuthorityName);
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
app.MapFallbackToFile("index.html");

Services.Get<MongoDBHandler>();
Services.Get<RSIRoadmapScraper>();

await app.RunAsync();