using System;
using System.IO;
using System.Net;

namespace UEESA.RSIScraper
{
    internal static class RSIStatusCheck
    {
        public static readonly Uri URL_RSI = new("https://robertsspaceindustries.com");
        public static readonly Uri URL_RSI_Transmissions = new("https://robertsspaceindustries.com/comm-link");
        public static readonly Uri URL_RSI_Spectrum = new("https://robertsspaceindustries.com/spectrum/community/SC");
        public static readonly Uri URL_RSI_Roadmap_ReleaseView = new("https://robertsspaceindustries.com/roadmap/release-view");
        public static readonly Uri URL_RSI_Roadmap_ProgressTracker = new("https://robertsspaceindustries.com/roadmap/release-view");
        public static readonly Uri URL_RSI_Telemetry = new("https://robertsspaceindustries.com/telemetry");
        public static readonly Uri URL_RSI_IssueCouncil = new("https://issue-council.robertsspaceindustries.com");
        public static readonly Uri URL_RSI_PledgeStore = new("https://robertsspaceindustries.com/pledge");
        public static readonly Uri URL_RSI_ServicesStatus = new("https://status.robertsspaceindustries.com");

        internal static ERSIStatus RSIStatus => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI));
        internal static ERSIStatus RSITransmissionsStatus => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_Transmissions));
        internal static ERSIStatus RSISpectrumStatus => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_Spectrum));
        internal static ERSIStatus RSIRoadmapReleaseViewStatus => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_Roadmap_ReleaseView));
        internal static ERSIStatus RSIRoadmapProgressTrackerStatus => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_Roadmap_ProgressTracker));
        internal static ERSIStatus RSITelemetryStatus => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_Telemetry));
        internal static ERSIStatus RSIIssueCouncilStatus => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_IssueCouncil));
        internal static ERSIStatus RSIPledgeStoreStatus => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_PledgeStore));
        internal static ERSIStatus RSIServicesStatusStatus => TranslateHTTPStatus(RetrieveStatusCode(URL_RSI_ServicesStatus));

        public enum ERSIStatus
        {
            Online,
            Maintenance,
            Malfunction,
            Unauthorized
        }

        private static ERSIStatus TranslateHTTPStatus(HttpStatusCode code) => code == HttpStatusCode.OK ? ERSIStatus.Online :
                   code == HttpStatusCode.Redirect || code == HttpStatusCode.RedirectKeepVerb || 
                        code == HttpStatusCode.RedirectMethod || code == HttpStatusCode.PermanentRedirect ||
                        code == HttpStatusCode.TemporaryRedirect || code == HttpStatusCode.ServiceUnavailable ? ERSIStatus.Maintenance :
                   code == HttpStatusCode.Unauthorized || code == HttpStatusCode.Forbidden ? ERSIStatus.Unauthorized :
                   ERSIStatus.Malfunction;

        private static HttpStatusCode RetrieveStatusCode(Uri statusURI)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(statusURI);
                request.Method = "GET";
                using var response = request.GetResponse();
                using var stream = response.GetResponseStream();
                using var reader = new StreamReader(stream); return ((HttpWebResponse)response).StatusCode;
            }
            catch (WebException e) 
            {
                switch (e.Status)
                {
                    case WebExceptionStatus.CacheEntryNotFound:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.ConnectFailure:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.ConnectionClosed:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.KeepAliveFailure:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.MessageLengthLimitExceeded:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.NameResolutionFailure:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.Pending:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.PipelineFailure:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.ProtocolError:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.ProxyNameResolutionFailure:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.ReceiveFailure:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.RequestCanceled:
                        return HttpStatusCode.ServiceUnavailable;
                    case WebExceptionStatus.RequestProhibitedByCachePolicy:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.RequestProhibitedByProxy:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.SecureChannelFailure:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.SendFailure:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.ServerProtocolViolation:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.Timeout:
                        return HttpStatusCode.NotFound;
                    case WebExceptionStatus.TrustFailure:
                        return HttpStatusCode.Unauthorized;
                    case WebExceptionStatus.UnknownError:
                        return HttpStatusCode.NotFound;
                    default:
                        return HttpStatusCode.NotFound;
                }
            }
        }
    }
}
