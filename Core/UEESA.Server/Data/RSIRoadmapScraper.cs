using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Bson;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UEESA.Server.Data.Json.RSI;
using UEESA.Shared.Data.Bson;
using UEESA.Server.Sockets.Handlers;

namespace UEESA.Server.Data
{
    internal class RSIRoadmapScraper
    {
        private MongoClient DB;
        private IMongoDatabase RSI_DB;

        private RSI_Bson_Roadmap roadmap_Data;
        internal RSI_Bson_Roadmap Roadmap_Data
        {
            get
            {
                return roadmap_Data;
            }

            set
            {
                roadmap_Data = value;
                new Action(() => Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(RSI_Bson_Roadmap).Name + JsonConvert.SerializeObject(value)) ).Invoke();
            }
        }

        public RSIRoadmapScraper()
        {
            Task.Run(Run);
        }

        internal async Task Run()
        {
            ConnectMongo();
            while (true)
            {
                await CheckForUpdate();
                await Task.Delay(TimeSpan.FromMinutes(2.5));
            }
        }

        private void ConnectMongo()
        {
            Logger.LogInfo("Connecting to UEESA Database...");
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(PrivateData.Instance.MongoDBConnectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            DB = new(settings);
            RSI_DB = DB.GetDatabase("rsidb");

            Logger.LogInfo("Connection to UEESA Database Successful!");
        }

        private async Task CheckForUpdate()
        {
            Logger.LogInfo("Checking for RSI update...");
            HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync("https://robertsspaceindustries.com/api/roadmap/v1/boards/1");
            JObject rsi_json = JObject.Parse(await response.Content.ReadAsStringAsync());
            RSI_Json_Roadmap parsed = JsonConvert.DeserializeObject<RSI_Json_Roadmap>(rsi_json["data"].ToString());

            DateTime updateDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(JsonConvert.DeserializeObject<long>(rsi_json["data"]["last_updated"].ToString()));

            if (DateTime.UtcNow.Day - updateDate.Day >= 14 || Globals.IsDevelopmentMode) ProcessData();
            else Logger.LogInfo("No RSI update available...");

            void ProcessData()
            {
                Logger.LogInfo("- New RSI update available!");
                Logger.LogInfo("| - Roadmap Update Date: " + updateDate.ToShortDateString() + " | " + updateDate.ToShortTimeString());
                ConvertRSIJsonToUsableBson();
            }

            void ConvertRSIJsonToUsableBson()
            {
                Logger.LogInfo("  | - Converting RSI Json to Mongo Bson...");

                RSI_Bson_Roadmap roadmap = new();
                roadmap.updated_datetime = updateDate;
                roadmap.releases = new();
                int relIndex = 0;
                int caIndex = 0;
                foreach (RSI_Json_Roadmap_Release rel in parsed.releases)
                {
                    RSI_Bson_Roadmap_Release release = new();
                    try
                    {
                        release.version = rel.name;
                        release.release_date = rel.description;
                        release.creation_datetime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(rel.time_created);
                        release.updated_datetime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(rel.time_created);
                        release.has_released = Convert.ToBoolean(rel.released);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.Message + " : " + ex.StackTrace);
                    }
                    release.cards = new();
                    foreach (RSI_Json_Roadmap_Card ca in rel.cards)
                    {
                        RSI_Bson_Roadmap_Card card = new();
                        try
                        {
                            card.name = ca.name;
                            card.description = ca.body;
                            card.category = (RSI_Bson_Roadmap_Card_Category)ca.category_id;
                            card.creation_datetime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(ca.time_created);
                            card.updated_datetime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(ca.time_modified);
                            card.thumnail_path = rsi_json["data"]["releases"][relIndex]["cards"][caIndex]["thumbnail"]["urls"]["source"].ToString();
                            card.status = Enum.Parse<RSI_Bson_Roadmap_Card_Status>(ca.status);
                            card.has_released = Convert.ToBoolean(ca.released);
                            card.teams = new();
                            //foreach (RSI_Json_Roadmap_Card_Teams tea in ca.teams.Values) card.teams.Add(tea.title);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex.Message + " : " + ex.StackTrace);
                        }
                        release.cards.Add(card);
                        caIndex++;
                    }
                    roadmap.releases.Add(release);
                    relIndex++;
                    caIndex = 0;
                }
                roadmap.releases.Reverse();
                Roadmap_Data = roadmap;

                Logger.LogInfo("  | - RSI Json Conversion to Mongo Bson Successful!");
                Logger.LogInfo("  | - Mongo Bson uploading to DB...");



                Logger.LogInfo("  | - Mongo Bson successfully uploaded!");
            }
        }
    }
}
