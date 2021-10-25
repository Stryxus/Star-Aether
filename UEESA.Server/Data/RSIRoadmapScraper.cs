using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

using UEESA.Server.Data.Json.RSI;
using UEESA.Server.Sockets.Handlers;
using UEESA.Data.Bson.Roadmap;
using UEESA.Data.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UEESA.Server.Data
{
    internal class RSIRoadmapScraper
    {
        private UEESA_Bson_Roadmap? roadmap_Data;
        internal UEESA_Bson_Roadmap? Roadmap_Data
        {
            get => roadmap_Data;

            set
            {
                roadmap_Data = value;
                Task.Run(() => Services.Get<StateSocketHandler>().SendMessageToAllAsync(new UEESA_Json_StateSocketDataCapsule<UEESA_Bson_Roadmap>
                {
                    attributes = new List<string>() { StateSocketDataCapsuleAttributes.GetRoadmapData.ToString() },
                    data = value
                }));
            }
        }

        public RSIRoadmapScraper()
        {
            Task.Run(Run);
        }

        internal async Task Run()
        {
            while (true)
            {
                await CheckForUpdate();
                await Task.Delay(TimeSpan.FromMinutes(2));
            }
        }

        private async Task CheckForUpdate()
        {
            Logger.LogInfo("Checking for RSI update...");
            HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync("https://robertsspaceindustries.com/api/roadmap/v1/boards/1");
            JObject rsiJson = JObject.Parse(await response.Content.ReadAsStringAsync());
            JToken? data = rsiJson is not null && rsiJson["data"] is not null ? rsiJson["data"] : null;
            JToken? lastUpdated = data is not null && data["last_updated"] is not null ? data["last_updated"] : null;

            if (data is not null && lastUpdated is not null)
            {
                RSI_Json_Roadmap? rsiParsedJson = JsonConvert.DeserializeObject<RSI_Json_Roadmap>(data.ToString());
                DateTime updateDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(JsonConvert.DeserializeObject<long>(lastUpdated.ToString()));

                if (rsiParsedJson is null) Logger.LogError("The RSI Scraper should ");
                else if (DateTime.UtcNow.Day - updateDate.Day >= 14 || Globals.IsDevelopmentMode || Roadmap_Data is null) ProcessRSIToUEESA(rsiParsedJson, updateDate);
                else Logger.LogInfo("No RSI update available...");
            }

            void ProcessRSIToUEESA(RSI_Json_Roadmap rsiParsedJson, DateTime updateDate)
            {
                Logger.LogInfo("- New RSI update available!");
                Logger.LogInfo("| - Roadmap Update Date: " + updateDate.ToShortDateString() + " | " + updateDate.ToShortTimeString());
                Logger.LogInfo("  | - Converting RSI Json to Mongo Bson...");

                UEESA_Bson_Roadmap roadmap = new();
                roadmap.UpdatedDateTime = updateDate;
                roadmap.Releases = new();
                int relIndex = 0;
                int caIndex = 0;

                if (rsiParsedJson is not null)
                {
                    foreach (RSI_Json_Roadmap_Release rel in rsiParsedJson.releases)
                    {
                        UEESA_Bson_Roadmap_Release release = new();

                        try
                        {
                            release.Version = rel.name;
                        }
                        catch (Exception)
                        {
                            Logger.LogError("Release Version value cannot be discerned from index '" + relIndex + "'");
                        }

                        try
                        {
                            release.ReleaseDate = rel.description;
                        }
                        catch (Exception)
                        {
                            Logger.LogError("Release Date value cannot be discerned from index '" + relIndex + "'");
                        }

                        try
                        {
                            release.CreationDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(rel.time_created);
                        }
                        catch (Exception)
                        {
                            Logger.LogError("Release Creation DateTime value cannot be discerned from index '" + relIndex + "'");
                        }

                        try
                        {
                            release.UpdatedDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(rel.time_modified);
                        }
                        catch (Exception)
                        {
                            Logger.LogError("Release Updated DateTime value cannot be discerned from index '" + relIndex + "'");
                        }

                        try
                        {
                            release.HasReleased = Convert.ToBoolean(rel.released);
                        }
                        catch (Exception)
                        {
                            Logger.LogError("Release Has Released value cannot be discerned from index '" + relIndex + "'");
                        }

                        release.Cards = new();

                        foreach (RSI_Json_Roadmap_Card ca in rel.cards)
                        {
                            UEESA_Bson_Roadmap_Card card = new();

                            try
                            {
                                card.Name = ca.name;
                            }
                            catch (Exception)
                            {
                                Logger.LogWarn("Card Name value cannot be discerned from index '" + relIndex + "' - '" + caIndex + "' - Defaulting value!");
                            }

                            try
                            {
                                card.Description = ca.body;
                            }
                            catch (Exception)
                            {
                                Logger.LogWarn("Card Description value cannot be discerned from index '" + relIndex + "' - '" + caIndex + "' - Defaulting value!");
                            }

                            try
                            {
                                card.Category = (UEESA_Bson_Roadmap_Card_Category)ca.category_id;
                            }
                            catch (Exception)
                            {
                                Logger.LogWarn("Card Category value cannot be discerned from index '" + relIndex + "' - '" + caIndex + "' - Defaulting value!");
                            }

                            try
                            {
                                card.CreationDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(ca.time_created);
                            }
                            catch (Exception)
                            {
                                Logger.LogWarn("Card Creation DateTime value cannot be discerned from index '" + relIndex + "' - '" + caIndex + "' - Defaulting value!");
                            }

                            try
                            {
                                card.UpdatedDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(ca.time_modified);
                            }
                            catch (Exception)
                            {
                                Logger.LogWarn("Card Updated DateTime value cannot be discerned from index '" + relIndex + "' - '" + caIndex + "' - Defaulting value!");
                            }

                            try
                            {
                                JToken? stepOne = data["releases"];
                                JToken? stepTwo = stepOne is not null ? stepOne[relIndex] : null;
                                JToken? stepThree = stepTwo is not null ? stepTwo["cards"] : null;
                                JToken? stepFour = stepThree is not null ? stepThree[caIndex] : null;
                                JToken? stepFive = stepFour is not null ? stepFour["thumbnail"] : null;
                                JToken? stepSix = stepFive is not null ? stepFive["urls"] : null;
                                JToken? stepSeven = stepSix is not null ? stepSix["source"] : null;
                                card.ThumnailURL = stepSeven is not null ? stepSeven.ToString() : null;
                            }
                            catch (Exception)
                            {
                                Logger.LogWarn("Card Thumbnail Path value cannot be discerned from index '" + relIndex + "' - '" + caIndex + "' - Defaulting value!");
                            }

                            try
                            {
                                card.Status = Enum.Parse<RSI_Bson_Roadmap_Card_Status>(ca.status);
                            }
                            catch (Exception)
                            {
                                Logger.LogWarn("Card Status value cannot be discerned from index '" + relIndex + "' - '" + caIndex + "' - Defaulting value!");
                            }

                            try
                            {
                                card.HasReleased = Convert.ToBoolean(ca.released);
                            }
                            catch (Exception)
                            {
                                Logger.LogWarn("Card Head Released value cannot be discerned from index '" + relIndex + "' - '" + caIndex + "' - Defaulting value!");
                            }

                            try
                            {
                                card.Teams = new();
                                JToken? stepOne = data["releases"];
                                JToken? stepTwo = stepOne is not null ? stepOne[relIndex] : null;
                                JToken? stepThree = stepTwo is not null ? stepTwo["cards"] : null;
                                JToken? stepFour = stepThree is not null ? stepThree[caIndex] : null;
                                JToken? stepFive = stepFour is not null ? stepFour["teams"] : null;
                                if (stepFive is not null)
                                {
                                    foreach (JToken team in stepFive.Children())
                                    {
                                        card.Teams.Add(team["abbreviation"] + " - " + (team["title"] is not null).ToString());
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                Logger.LogWarn("Card Teams List value cannot be discerned from index '" + relIndex + "' - '" + caIndex + "' - Defaulting value!");
                            }

                            release.Cards.Add(card);
                            caIndex++;
                        }

                        roadmap.Releases.Add(release);
                        relIndex++;
                        caIndex = 0;
                    }
                }

                roadmap.Releases.Reverse();
                Roadmap_Data = roadmap;

                Logger.LogInfo("  | - RSI Json Conversion to Mongo Bson Successful!");
                Logger.LogInfo("  | - Mongo Bson uploading to DB...");



                Logger.LogInfo("  | - Mongo Bson successfully uploaded!");
            }
        }
    }
}
