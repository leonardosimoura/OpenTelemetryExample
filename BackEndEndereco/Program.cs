using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace BackEndEndereco
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddOpenTelemetry(options =>
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
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }


}
