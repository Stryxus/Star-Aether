using System;
using System.Net;

namespace UEESA.RSIScraper
{
    internal static class RSIStatusCheck
    {
        private static readonly Uri URL_RSI = new("https://robertsspaceindustries.com/");
        private static readonly Uri URL_RSI_Transmissions = new("https://robertsspaceindustries.com/comm-link");
        private static readonly Uri URL_RSI_Spectrum = new("https://robertsspaceindustries.com/spectrum/community/SC");
        private static readonly Uri URL_RSI_Roadmap = new("https://robertsspaceindustries.com/roadmap/release-view");
        private static readonly Uri URL_RSI_Telemetry = new("https://robertsspaceindustries.com/telemetry");
        private static readonly Uri URL_RSI_IssueCouncil = new("https://issue-council.robertsspaceindustries.com/");
        private static readonly Uri URL_RSI_PledgeStore = new("https://robertsspaceindustries.com/pledge");
        private static readonly Uri URL_RSI_ServicesStatus = new("https://status.robertsspaceindustries.com/");

        internal static RSIStatus IsRSIWorking => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI));
        internal static RSIStatus ISRSITransmissionsWorking => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_Transmissions));
        internal static RSIStatus ISRSISpectrumWorking => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_Spectrum));
        internal static RSIStatus ISRSIRoadmapWorking => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_Roadmap));
        internal static RSIStatus ISRSITelemetryWorking => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_Telemetry));
        internal static RSIStatus ISRSIIssueCouncilWorking => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_IssueCouncil));
        internal static RSIStatus ISRSIPledgeStoreWorking => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_PledgeStore));
        internal static RSIStatus ISRSIServicesStatusWorking => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_ServicesStatus));

        public enum RSIStatus
        {
            Online,
            Maintenance,
            Malfunction,
            Unauthorized
        }

        private static RSIStatus TranslateHTTPStatus(HttpStatusCode code)
        {
            return code == HttpStatusCode.OK ? RSIStatus.Online :
                   code == HttpStatusCode.Redirect || code == HttpStatusCode.RedirectKeepVerb || code == HttpStatusCode.RedirectMethod ||
                        code == HttpStatusCode.PermanentRedirect || code == HttpStatusCode.TemporaryRedirect ? RSIStatus.Maintenance :
                   code == HttpStatusCode.Unauthorized || code == HttpStatusCode.Forbidden ? RSIStatus.Unauthorized :
                   RSIStatus.Malfunction;
        }

        private static HttpStatusCode RetrieveStatusCode(Uri statusURI)
        {
            HttpStatusCode result = default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(statusURI);
            request.Method = "HEAD";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response != null)
                {
                    result = response.StatusCode;
                    response.Close();
                }
            }
            return result;
        }
    }
}
