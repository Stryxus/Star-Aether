using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using SA.Web.Server.WebSockets;
using SA.Web.Shared.Json;

using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace SA.Web.Server
{
    internal class MongoDBInterface
    {
        private MongoClient Client;
        private IMongoDatabase Database_Public_Data;

        internal event Action OnPublicDataUpdate;
        internal void InvokeOnPublicDataUpdate() => OnPublicDataUpdate?.Invoke();

        internal async Task Connect()
        {
            if (Client == null)
            {
                await Logger.LogInfo("Connecting to MongoDB...");
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(PrivateData.Instance.MongoDBConnectionString));
                settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
                settings.ConnectTimeout = TimeSpan.FromSeconds(5);
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(2.5);
                settings.DirectConnection = true;
                Client = new MongoClient(settings);
                Database_Public_Data = Client.GetDatabase("public_data");
                await Logger.LogInfo("Connected to MongoDB!");

                OnPublicDataUpdate += async () =>
                {
                    Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(NewsData).Name + await GetNewsData());
                    Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(RoadmapData).Name + await GetRoadmapData());
                    Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(ChangelogData).Name + await GetChangelogData());
                };
            }
        }

        internal async Task<string> GetNewsData()
        {
            NewsData data = new NewsData { NewsPosts = new List<NewsEntryData>() };
            try
            {
                foreach (BsonDocument entry in await Database_Public_Data.GetCollection<BsonDocument>("news_data").AsQueryable().ToListAsync()) data.NewsPosts.Add(BsonSerializer.Deserialize<NewsEntryData>(entry));
            } catch (TimeoutException)
            {
                await Logger.LogWarn("News data request timed out - Using default data structure.");
            } catch (NullReferenceException)
            {
                await Logger.LogError("News data request threw null - Using default data structure.");
            }
            return JsonConvert.SerializeObject(data);
        }

        internal async Task<string> GetRoadmapData()
        {
            RoadmapData data = new RoadmapData { Cards = new List<RoadmapCardData>() };
            try
            {
                foreach (BsonDocument entry in await Database_Public_Data.GetCollection<BsonDocument>("roadmap_data").AsQueryable().ToListAsync()) data.Cards.Add(BsonSerializer.Deserialize<RoadmapCardData>(entry));
            }
            catch (TimeoutException)
            {
                await Logger.LogWarn("Roadmap data request timed out - Using default data structure.");
            }
            catch (NullReferenceException)
            {
                await Logger.LogError("Roadmap data request threw null - Using default data structure.");
            }
            return JsonConvert.SerializeObject(data);
        }

        internal async Task<string> GetChangelogData()
        {
            ChangelogData data = new ChangelogData { ChangelogPosts = new List<ChangelogEntryData>() };
            try
            {
                foreach (BsonDocument entry in await Database_Public_Data.GetCollection<BsonDocument>("changelog_data").AsQueryable().ToListAsync()) data.ChangelogPosts.Add(BsonSerializer.Deserialize<ChangelogEntryData>(entry));
            }
            catch (TimeoutException)
            {
                await Logger.LogWarn("Changelog data request timed out - Using default data structure.");
            }
            catch (NullReferenceException)
            {
                await Logger.LogError("Changelog data request threw null - Using default data structure.");
            }
            return JsonConvert.SerializeObject(data);
        }
    }
}
