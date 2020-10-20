using System;
using System.Net;

using Newtonsoft.Json;

namespace SA.Web.Server
{
    internal static class ServerLocator
    {
        internal static bool IsUKServer()
        {
            using (WebClient client = new WebClient())
            {
                IpInfo ipInfo = JsonConvert.DeserializeObject<IpInfo>(client.DownloadString("http://ipinfo.io"));
                return ipInfo.Country == "GB";
            }
        }

        public class IpInfo
        {
            public string Country { get; set; }
        }
    }
}
