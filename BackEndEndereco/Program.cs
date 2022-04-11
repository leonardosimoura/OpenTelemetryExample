using BackEndEndereco;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;


AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

const string OtlpExporterEndpoint = "http://localhost:8200";

builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    options.ParseStateValues = true;
    options.AddOtlpExporter(configure =>
    {
        configure.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        configure.Endpoint = new Uri("http://localhost:8200");
        //configure.Headers = "Authorization=Bearer {apm_secret_token}";
    });
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackEndEndereco", Version = "v1" });
});

builder.Services.AddOpenTelemetryTracing((builder) => builder
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BackEndEndereco"))
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .AddMongoDBInstrumentation()
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
    })
    );

builder.Services.AddScoped<IMongoClient>((srv) =>
{
    var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(builder.Configuration.GetConnectionString("BackEndEnderecoContext")));
    var options = new InstrumentationOptions { CaptureCommandText = true };
    clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber(options));
    var mongoClient = new MongoClient(clientSettings);
    return mongoClient;
});

BsonClassMap.RegisterClassMap<Endereco>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
    cm.MapIdMember(c => c.EnderecoId);
});

builder.Services.AddScoped<IMongoDatabase>((srv) => srv.GetService<IMongoClient>().GetDatabase("backend_endereco"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackEndEndereco v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();