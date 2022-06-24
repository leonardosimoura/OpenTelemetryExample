using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Diagnostics;

namespace FrontEnd.SerilogConfiguration
{
    public class DataDogEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (Activity.Current != null)
            {
                var stringTraceId = Activity.Current.TraceId.ToString();
                var stringSpanId = Activity.Current.SpanId.ToString();

                var ddTraceId = Convert.ToUInt64(stringTraceId.Substring(16), 16).ToString();
                var ddSpanId = Convert.ToUInt64(stringSpanId, 16).ToString();

                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("dd.trace_id", ddTraceId));
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("dd.span_id", ddSpanId));
            }
        }
    }
}
