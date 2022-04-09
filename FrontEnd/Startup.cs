using FrontEnd.Tracing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FrontEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            services.AddOpenTelemetryTracing((builder) => builder
               .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                    .AddService("FrontEnd",serviceVersion: "1.0")
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
                    configure.Endpoint = new Uri("http://localhost:8200");
                    //configure.Headers = "Authorization=Bearer {apm_secret_token}";

               })
               .AddConsoleExporter());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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
        }
    }
}
