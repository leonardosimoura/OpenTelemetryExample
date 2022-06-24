using BackEndPessoa.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Shared;
using System;
using System.Net;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

const string OtlpExporterEndpoint = "http://192.168.1.100:4317";

builder.Host.UseSerilog(SerilogConfig.CreateSerilogLogger());

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

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackEndPessoa", Version = "v1" });
});

builder.Services.AddDbContext<BackEndPessoaContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("BackEndPessoaContext")));


builder.Services.AddOpenTelemetryTracing((builder) => builder
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BackEndPessoa"))
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .AddEntityFrameworkCoreInstrumentation((config) =>
    {
        config.SetDbStatementForStoredProcedure = true;
        config.SetDbStatementForText = true;
    })
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

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackEndPessoa v1"));

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();