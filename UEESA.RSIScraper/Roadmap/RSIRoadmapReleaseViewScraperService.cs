using System;
using System.Threading.Tasks;

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
                Browser.ExecuteScriptAsync("document.getElementsByClassName('TogglePreviousReleases-yixp65-1')[0].click();", true);
                Browser.ExecuteScriptAsync("document.getElementsByClassName('Button-sc-1i76va4-2')[0].click();", true);
                for (int i = 0; i < 12; i++) Browser.ExecuteScriptAsync("document.getElementsByClassName('ReleaseHeader-xqp955-3')[" + i + "].click();", true);
                HTML.LoadHtml(await Browser.GetSourceAsync());
                foreach (HtmlNode cardBody in HTML.DocumentNode.SelectSingleNode(".//div[contains(@class, 'Board__Releases-c7lmub-7')]").SelectNodes(".//section[contains(@class, 'Release__Wrapper-sc-1y9ya50-0')]"))
                {
                    await Logger.LogInfo("- Release: " + cardBody.SelectSingleNode(".//h2[contains(@class, 'ReleaseHeader__ReleaseHeaderName-xqp955-1')]").InnerText);
                    foreach (HtmlNode category in cardBody.SelectNodes(".//section[contains(@class, 'Category__Wrapper-sc-3z36kz-0')]"))
                    {
                        await Logger.LogInfo("| - Category Name: " + category.SelectSingleNode(".//h2[contains(@class, 'Category__CategoryName-sc-3z36kz-4')]").InnerText);
                        foreach (HtmlNode feature in category.SelectNodes(".//section[contains(@class, 'Card__Wrapper-a2fcbm-0')]"))
                        {
                            await Logger.LogInfo("  | Release Feature: " + feature.SelectSingleNode(".//header[contains(@class, 'Card__TitleBar-a2fcbm-4')]/h3").InnerText + " - " + feature.SelectSingleNode(".//p[contains(@class, 'Card__Description-a2fcbm-6')]").InnerText);
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
