using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

using UEESA.Server.Data.Bson;

namespace UEESA.Server.Data
{
    public class RSIRoadmapScraper
    {
        private Uri RSI_Roadmap_JSON_Link = new("https://robertsspaceindustries.com/api/roadmap/v1/boards/1");
        private MongoClient DB;
        private IMongoDatabase RSI_DB;
        private IMongoCollection<RSI_State_Indicator> RSI_STATE_INDICATORS_COLLECTION;

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
                await Task.Delay(TimeSpan.FromMinutes(2));
            }
        }

        private void ConnectMongo()
        {
            Logger.LogInfo("Connecting to UEESA Database...");
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(PrivateData.Instance.MongoDBConnectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            DB = new(settings);
            RSI_DB = DB.GetDatabase("rsidb");
            RSI_STATE_INDICATORS_COLLECTION = RSI_DB.GetCollection<RSI_State_Indicator>("rsi_state_indicators");
            Logger.LogInfo("Connection to UEESA Successful!");
        }

        private async Task CheckForUpdate()
        {
            Logger.LogInfo("Checking for RSI update...");
            HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(RSI_Roadmap_JSON_Link);
            JObject jo = JObject.Parse(await response.Content.ReadAsStringAsync());
            string description = (string)jo["data"]["description"];

            int livePos = description.IndexOf("Live Version:") + "Live Version:".Length + 1;
            int ptuPos = description.IndexOf("PTU Version:") + "PTU Version".Length + 1;
            int datePos = description.IndexOf("Latest Roadmap Roundup:") + "Latest Roadmap Roundup:".Length + 1;

            if (await RSI_STATE_INDICATORS_COLLECTION.CountDocumentsAsync(bson => true) > 0)
            {
                List<RSI_State_Indicator> indicators = await RSI_STATE_INDICATORS_COLLECTION.Find(bson => true).ToListAsync();
                RSI_State_Indicator latestIndicator = indicators.OrderBy(x => x.change_date.TimeOfDay).Last();
                if (latestIndicator.change_date < DateTime.ParseExact(description.Substring(datePos, 10), "MM/dd/yyyy", CultureInfo.InvariantCulture).ToUniversalTime() ||
                    latestIndicator.live_version != description.Substring(livePos, description[livePos..].IndexOf(' ')) ||
                    latestIndicator.ptu_version != description[ptuPos..]) await CreateNewIndicatorEntry();
                else Logger.LogInfo("No RSI update available...");
            }
            else await CreateNewIndicatorEntry();

            async Task CreateNewIndicatorEntry()
            {
                RSI_State_Indicator ind = new();
                ind.Id = ObjectId.GenerateNewId();
                ind.change_date = DateTime.ParseExact(description.Substring(datePos, 10), "MM/dd/yyyy", CultureInfo.InvariantCulture).ToUniversalTime();
                ind.live_version = description.Substring(livePos, description[livePos..].IndexOf(' '));
                ind.ptu_version = description[ptuPos..];
                await RSI_STATE_INDICATORS_COLLECTION.InsertOneAsync(ind);
                Logger.LogInfo("New RSI update available:");
                Logger.LogInfo(" - Roadmap Update Date: " + ind.change_date.ToShortDateString() + " | " + ind.change_date.ToShortTimeString());
                Logger.LogInfo(" - Live Update Version: " + ind.live_version);
                Logger.LogInfo(" - PTU Update Version: " + ind.ptu_version);
            }
        }
    }
}
