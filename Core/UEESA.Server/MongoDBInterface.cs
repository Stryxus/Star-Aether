using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using UEESA.Server.WebSockets;
using UEESA.Json.Roadmap;
using UEESA.RSIScraper.Roadmap;

using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace UEESA.Server
{
    internal class MongoDBInterface
    {
        private MongoClient Client;
        private IMongoDatabase Database_Public_Data;

        internal void Connect()
        {
            if (Client == null)
            {
                Logger.LogInfo("Connecting to MongoDB...");
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(PrivateData.Instance.MongoDBConnectionString));
                settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
                settings.ConnectTimeout = TimeSpan.FromSeconds(5);
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(2.5);
                settings.DirectConnection = true;
                Client = new MongoClient(settings);
                Database_Public_Data = Client.GetDatabase("public_data");
                Logger.LogInfo("Connected to MongoDB!");

                Services.Get<RSIRoadmapReleaseViewScraperService>().OnRaodmapReleaseViewStateChange += () =>
                {
                    Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(RSI_Roadmap_State).Name + GetRoadmapData());
                };
            }
        }

        internal string GetRoadmapData()
        {
            RSI_Roadmap_State data = new RSI_Roadmap_State { Features = new() };
            try
            {
                //foreach (BsonDocument entry in await Database_Public_Data.GetCollection<BsonDocument>("roadmap_data").AsQueryable().ToListAsync()) data.Cards.Add(BsonSerializer.Deserialize<RoadmapCardData>(entry));
            }
            catch (TimeoutException)
            {
                Logger.LogWarn("Roadmap data request timed out - Using default data structure.");
            }
            catch (NullReferenceException)
            {
                Logger.LogError("Roadmap data request threw null - Using default data structure.");
            }
            return JsonConvert.SerializeObject(Services.Get<RSIRoadmapReleaseViewScraperService>().State);
        }
    }
}
