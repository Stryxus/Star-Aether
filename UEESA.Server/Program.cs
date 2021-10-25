﻿using System.IO.Compression;
using System.Linq;
using System.Security.Authentication;

using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;

using UEESA.Server.Data;
using UEESA.Server.Sockets;
using UEESA.Server.Sockets.Handlers;

#if RELEASE
string CORSAuthorityName = "_starAetherCORSAuthority";
#endif

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
Services.SetConfiguration(builder.Configuration);

builder.WebHost.UseKestrel(ko => ko.ConfigureHttpsDefaults(o => o.SslProtocols = SslProtocols.Tls13));
#if DEBUG
builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { ConnectionString = "00000000-0000-0000-0000-000000000000" });
#else
//builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { ConnectionString = Services.Configuration["APPINSIGHTS_CONNECTIONSTRING"] });
#endif
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApi(Services.Configuration.GetSection("AZURE_AD_B2C"));
#if RELEASE
builder.Services.AddCors(options => options.AddPolicy(name: CORSAuthorityName, builder => builder.WithOrigins("https://staraether.com")));
#endif
builder.Services.AddResponseCompression(o =>
{
    o.Providers.Add<BrotliCompressionProvider>();
    o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
});
builder.Services.AddResponseCaching();
builder.Services.AddWebSocketManager();
builder.Services.AddSingleton<MongoDBHandler>(new MongoDBHandler());
builder.Services.AddSingleton<RSIRoadmapScraper>(new RSIRoadmapScraper());
builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, o => o.TokenValidationParameters.NameClaimType = "name");
builder.Services.Configure<ForwardedHeadersOptions>(o => o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.SmallestSize);

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
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });
app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseWebSockets();
app.MapWebSocketManager("/state", Services.Get<StateSocketHandler>());
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
#if RELEASE
app.UseCors(CORSAuthorityName);
#endif
app.UseResponseCaching();
app.UseEndpoints(endpoints => 
{
    endpoints.MapFallbackToFile("index.html");
});

Services.Get<MongoDBHandler>();
Services.Get<RSIRoadmapScraper>();

await app.RunAsync();