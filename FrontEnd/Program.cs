using FrontEnd.Tracing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Net;
using System.Text.Json;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

const string OtlpExporterEndpoint = "http://localhost:8200";

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5053, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});


builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    options.ParseStateValues = true;
    options.AddOtlpExporter(configure =>
    {
        configure.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        configure.Endpoint = new Uri(OtlpExporterEndpoint);
        //configure.Headers = "Authorization=Bearer {apm_secret_token}";
    });
});


builder.Services.AddRazorPages()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddOpenTelemetryTracing((builder) => builder
   .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("FrontEnd", serviceVersion: "1.0")
                        )
   .AddSource(TracingHelper.KEY)
   .AddAspNetCoreInstrumentation()
   .AddHttpClientInstrumentation()
   .AddZipkinExporter(o =>
   {
       o.Endpoint = new Uri(@"http://localhost:9411/api/v2/spans");
   })
   .AddJaegerExporter(config =>
   {
       config.AgentHost = "localhost";
       config.AgentPort = 6831;
   })
   .AddOtlpExporter(configure =>
   {
       configure.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
       configure.Endpoint = new Uri(OtlpExporterEndpoint);
       //configure.Headers = "Authorization=Bearer {apm_secret_token}";
   }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

app.Run();