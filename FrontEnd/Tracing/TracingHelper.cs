using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEnd.Tracing
{
    public class TracingHelper
    {
        public const string KEY = "MyActivitySource";

        public static ActivitySource ActivitySource { get; } = new ActivitySource(KEY);
        static TracingHelper()
        {
            var activityListener = new ActivityListener
            {
                ShouldListenTo = s => true,
                SampleUsingParentId = (ref ActivityCreationOptions<string> activityOptions) => ActivitySamplingResult.AllData,
                Sample = (ref ActivityCreationOptions<ActivityContext> activityOptions) => ActivitySamplingResult.AllData
            };
            ActivitySource.AddActivityListener(activityListener);
        }
    }
}
