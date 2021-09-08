﻿using System.Security.Authentication;

using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;

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
#else
builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { ConnectionString = Services.Configuration["APPINSIGHTS_CONNECTIONSTRING"] });
#endif
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORSAuthorityName,
                      builder =>
                      {
#if DEBUG
                          builder.WithOrigins("https://localhost:5001");
#else
                          builder.WithOrigins("https://staraether.com");
#endif
                      });
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

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

FileExtensionContentTypeProvider provider = new();
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
app.MapFallbackToFile("index.html");

Services.Get<MongoDBHandler>();
Services.Get<RSIRoadmapScraper>();

await app.RunAsync();