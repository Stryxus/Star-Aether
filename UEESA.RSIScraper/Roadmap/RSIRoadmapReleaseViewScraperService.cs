using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using UEESA.Json.Roadmap;

using CefSharp;

using HtmlAgilityPack;

namespace UEESA.RSIScraper.Roadmap
{
    public class RSIRoadmapReleaseViewScraperService : RSIScraper
    {
        public RSIRoadmapReleaseViewScraperService() : base(RSIStatusCheck.URL_RSI_Roadmap_ReleaseView, TimeSpan.FromSeconds(5)) { }

        public event Action OnRaodmapReleaseViewStateChange;
        public RSI_Roadmap_State State { get; private set; } = new RSI_Roadmap_State
        {
            Features = new(),
            ReleaseStatus = new()
        };

        protected override async Task ParseData()
        {
            if (RSIStatusCheck.ISRSIRoadmapReleaseViewWorking == RSIStatusCheck.RSIStatus.Online)
            {
                await Logger.LogInfo("Running RSI Scrape: " + RSIStatusCheck.URL_RSI_Roadmap_ReleaseView.ToString());

                try
                {
                    Browser.ExecuteScriptAsync("document.getElementsByClassName('TogglePreviousReleases-yixp65-1')[0].click();", true);
                    Browser.ExecuteScriptAsync("document.getElementsByClassName('Button-sc-1i76va4-2')[0].click();", true);
                    for (int i = 0; i < 12; i++) Browser.ExecuteScriptAsync("document.getElementsByClassName('ReleaseHeader-xqp955-3')[" + i + "].click();", true);
                    HTML.LoadHtml(await Browser.GetSourceAsync());

                    foreach (HtmlNode cardBody in HTML.DocumentNode.Descendants().First(x => x.Name == "div" && x.HasClass("Board__Releases-c7lmub-7"))
                                                                   .Descendants().Where(x => x.Name == "section" && x.HasClass("Release__Wrapper-sc-1y9ya50-0")))
                    {
                        string currentRelease = cardBody.Descendants().First(x => x.Name == "h2" && x.HasClass("ReleaseHeader__ReleaseHeaderName-xqp955-1")).InnerText;
                        if (!State.Features.ContainsKey(currentRelease)) State.Features.Add(currentRelease, new Dictionary<RSI_Roadmap_State_Category, List<RSI_Roadmap_State_Feature>>());
                        string cardStatus = cardBody.Descendants().First(x => x.Name == "p" && x.HasClass("ReleaseHeader__ReleaseHeaderStatus-xqp955-2")).Descendants().First(x => x.Name == "strong").InnerText;
                        State.ReleaseStatus.Add(currentRelease, Enum.Parse<RSI_Roadmap_State_Status>(cardStatus, true));

                        foreach (HtmlNode category in cardBody.Descendants().Where(x => x.Name == "section" && x.HasClass("Category__Wrapper-sc-3z36kz-0")))
                        {
                            string currentCategoryString = category.Descendants().First(x => x.Name == "h2" && x.HasClass("Category__CategoryName-sc-3z36kz-4")).InnerText;
                            RSI_Roadmap_State_Category currentCategory =
                                currentCategoryString == "Characters" ? RSI_Roadmap_State_Category.Characters :
                                currentCategoryString == "Locations" ? RSI_Roadmap_State_Category.Locations :
                                currentCategoryString == "AI" ? RSI_Roadmap_State_Category.AI :
                                currentCategoryString == "Gameplay" ? RSI_Roadmap_State_Category.Gameplay :
                                currentCategoryString == "Ships and Vehicles" ? RSI_Roadmap_State_Category.Vehicles :
                                currentCategoryString == "Weapons and Items" ? RSI_Roadmap_State_Category.Items :
                                currentCategoryString == "Core Tech" ? RSI_Roadmap_State_Category.CoreTech :
                                                                                  RSI_Roadmap_State_Category.CoreTech;

                            if (!State.Features[currentRelease].ContainsKey(currentCategory)) State.Features[currentRelease].Add(currentCategory, new List<RSI_Roadmap_State_Feature>());

                            foreach (HtmlNode feature in category.Descendants().Where(x => x.Name == "a" && x.HasClass("Card__StyledNavLink-a2fcbm-2")))
                            {
                                List<string> teams = new();
                                List<HtmlNode> teamsNodeLookup = feature.Descendants().Where(x => x.Name == "section" && x.HasClass("Card__Teams-a2fcbm-7")).ToList();
                                if (teamsNodeLookup.Count > 0)
                                {
                                    List<HtmlNode> teamsNodes = feature.Descendants().First(x => x.Name == "section" && x.HasClass("Card__Teams-a2fcbm-7")).Descendants().ToList();
                                    if (teamsNodes.Count > 0)
                                    {
                                        foreach (HtmlNode team in teamsNodes) teams.Add(team.InnerText);
                                    }
                                }

                                List<HtmlNode> descriptionNodes = feature.Descendants().Where(x => x.Name == "p" && x.HasClass("Card__Description-a2fcbm-6")).ToList();
                                List<HtmlNode> imageNodes = feature.Descendants().Where(x => x.Name == "figure" && x.HasClass("Card__Thumbnail-a2fcbm-5")).ToList();
                                string status = feature.Descendants().First().Descendants().First(x => x.Name == "span").InnerText;
                                string thumbnailLink = imageNodes.Count > 0 ? imageNodes.First().GetAttributeValue("media", string.Empty) : null;

                                State.Features[currentRelease][currentCategory].Add(new RSI_Roadmap_State_Feature
                                {
                                    Name = feature.Descendants().First(x => x.Name == "header" && x.HasClass("Card__TitleBar-a2fcbm-4")).Descendants().First().InnerText,
                                    Description = descriptionNodes.Count > 0 ? descriptionNodes.First().InnerText : null,
                                    Status = Enum.Parse<RSI_Roadmap_State_Status>(status, true),
                                    Teams = teams,
                                    ThumbnailLink = imageNodes.Count > 0 ? thumbnailLink.StartsWith("https://media.robertsspaceindustries.com") ? thumbnailLink : RSIStatusCheck.URL_RSI + thumbnailLink : null,
                                });
                            }
                        }
                    }
                    State.Features = State.Features.Reverse().ToDictionary(x => x.Key, x => x.Value);
                    State.ReleaseStatus = State.ReleaseStatus.Reverse().ToDictionary(x => x.Key, x => x.Value);
                    OnRaodmapReleaseViewStateChange.Invoke();
                    await Logger.LogInfo("Completed RSI Scrape: " + RSIStatusCheck.URL_RSI_Roadmap_ReleaseView.ToString() + "\nReleases: " + State.Features.Keys.Count + "\nFeatures: " + State.Features.Values.Sum(x => x.Sum(o => o.Value.Count)));
                }
                catch (NullReferenceException e)
                {
                    await Logger.LogError("The RSI Roadmap Release View Scraper Service threw a " + e.GetType().Name + "!\n  - Message: " + e.Message + "\n  - StackTrace: " + e.StackTrace);
                }
                catch (InvalidOperationException e)
                {
                    await Logger.LogError("The RSI Roadmap Release View Scraper Service threw a " + e.GetType().Name + "!\n  - Message: " + e.Message + "\n  - StackTrace: " + e.StackTrace);
                }
                catch (ArgumentNullException e)
                {
                    await Logger.LogError("The RSI Roadmap Release View Scraper Service threw a " + e.GetType().Name + "!\n  - Message: " + e.Message + "\n  - StackTrace: " + e.StackTrace);
                }
                catch (ArgumentException e)
                {
                    await Logger.LogError("The RSI Roadmap Release View Scraper Service threw a " + e.GetType().Name + "!\n  - Message: " + e.Message + "\n  - StackTrace: " + e.StackTrace);
                }
            }
        }

        protected override async Task UploadToDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
