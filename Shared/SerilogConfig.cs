using Serilog;
using Shared.SerilogEnrichers;

namespace Shared
{
    public class SerilogConfig
    {
        public static ILogger CreateSerilogLogger()
        {
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.With<DataDogEnricher>()
                .WriteTo.Console(formatter: new Serilog.Formatting.Json.JsonFormatter(renderMessage: true))
                .CreateLogger();
        }
    }
}