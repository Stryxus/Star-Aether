using System;
using System.Threading.Tasks;

using CefSharp;

using HtmlAgilityPack;

namespace UEESA.RSIScraper.Roadmap
{
    public class RSIRoadmapProgressTrackerScraperService : RSIScraper
    {
        public RSIRoadmapProgressTrackerScraperService() : base (RSIStatusCheck.URL_RSI_Roadmap_ProgressTracker, TimeSpan.FromSeconds(5)) { }

        protected override async Task ParseData()
        {

        }

        protected override async Task UploadToDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
