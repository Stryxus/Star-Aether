using System;

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace SA.Web.Server
{
    public class InsightsFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public InsightsFilter(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetry item)
        {
            if (!OKtoSend(item)) { return; }
            if (item is RequestTelemetry request && request.Name != null)
            {
                if (request.Url.OriginalString.EndsWith("/state")) return;
            }
            this.Next.Process(item);
        }

        private bool OKtoSend(ITelemetry item)
        {
            if (!(item is DependencyTelemetry dependency)) return true;
            return dependency.Success != true;
        }
    }
}
