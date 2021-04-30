using System;
using System.Linq;
using System.Threading.Tasks;

using UEESA.Json.Roadmap;

using CefSharp;

using HtmlAgilityPack;

namespace UEESA.RSIScraper.Roadmap
{
    public class RSIRoadmapReleaseViewScraperService : RSIScraper
    {
        public RSIRoadmapReleaseViewScraperService() : base(RSIStatusCheck.URL_RSI_Roadmap_ReleaseView, TimeSpan.FromSeconds(5)) { }

        protected override async Task ParseData()
        {
            if (RSIStatusCheck.ISRSIRoadmapReleaseViewWorking == RSIStatusCheck.RSIStatus.Online)
            {
                RSI_Roadmap_State state = new RSI_Roadmap_State();

                Browser.ExecuteScriptAsync("document.getElementsByClassName('TogglePreviousReleases-yixp65-1')[0].click();", true);
                Browser.ExecuteScriptAsync("document.getElementsByClassName('Button-sc-1i76va4-2')[0].click();", true);
                for (int i = 0; i < 12; i++) Browser.ExecuteScriptAsync("document.getElementsByClassName('ReleaseHeader-xqp955-3')[" + i + "].click();", true);
                HTML.LoadHtml(await Browser.GetSourceAsync());
                foreach (HtmlNode cardBody in HTML.DocumentNode.Descendants().Where(x => x.Name == "div" && x.HasClass("Board__Releases-c7lmub-7")).First()
                    .Descendants().Where(x => x.Name == "section" && x.HasClass("Release__Wrapper-sc-1y9ya50-0")))
                {
                    await Logger.LogInfo("- Release: " + cardBody.Descendants().Where(x => x.Name == "h2" && x.HasClass("ReleaseHeader__ReleaseHeaderName-xqp955-1")).First().InnerText);
                    foreach (HtmlNode category in cardBody.Descendants().Where(x => x.Name == "section" && x.HasClass("Category__Wrapper-sc-3z36kz-0")))
                    {
                        await Logger.LogInfo("| - Category Name: " + category.Descendants().Where(x => x.Name == "h2" && x.HasClass("Category__CategoryName-sc-3z36kz-4")).First().InnerText);
                        foreach (HtmlNode feature in category.Descendants().Where(x => x.Name == "section" && x.HasClass("Card__Wrapper-a2fcbm-0")))
                        {
                            await Logger.LogInfo("  | Release Feature: " + feature.Descendants().Where(x => x.Name == "header" && x.HasClass("Card__TitleBar-a2fcbm-4")).First().InnerText + " - " + 
                                feature.Descendants().Where(x => x.Name == "p" && x.HasClass("Card__Description-a2fcbm-6")).First().InnerText);
                        }
                    }
                }
            }
        }

        protected override async Task UploadToDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
