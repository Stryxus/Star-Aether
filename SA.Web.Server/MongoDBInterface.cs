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
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(""));
                settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls13 };
                Client = new MongoClient(settings);
                Database_Public_Data = Client.GetDatabase("public_data");
                await Logger.LogInfo("Connected to MongoDB!");

                OnPublicDataUpdate += async () =>
                {
                    Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(NewsData).Name + await GetNewsData());
                    Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(RoadmapData).Name + await GetRoadmapData());
                    Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(ChangelogData).Name + await GetChangelogData());
                    Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(MediaPhotographyData).Name + await GetPhotographyData());
                };
            }
        }

        internal async Task<string> GetNewsData()
        {
            NewsData data = new NewsData { NewsPosts = new List<NewsEntryData>() };
            foreach (BsonDocument entry in await (await Database_Public_Data.GetCollection<BsonDocument>("news-data").FindAsync(_ => true)).ToListAsync()) data.NewsPosts.Add(BsonSerializer.Deserialize<NewsEntryData>(entry));
            return JsonConvert.SerializeObject(data);
        }

        internal async Task<string> GetRoadmapData()
        {
            RoadmapData data = new RoadmapData { Cards = new List<RoadmapCardData>() };
            foreach (BsonDocument entry in await (await Database_Public_Data.GetCollection<BsonDocument>("roadmap-data").FindAsync(_ => true)).ToListAsync()) data.Cards.Add(BsonSerializer.Deserialize<RoadmapCardData>(entry));
            return JsonConvert.SerializeObject(data);
        }

        internal async Task<string> GetChangelogData()
        {
            ChangelogData data = new ChangelogData { ChangelogPosts = new List<ChangelogEntryData>() };
            foreach (BsonDocument entry in await (await Database_Public_Data.GetCollection<BsonDocument>("changelog-data").FindAsync(_ => true)).ToListAsync()) data.ChangelogPosts.Add(BsonSerializer.Deserialize<ChangelogEntryData>(entry));
            return JsonConvert.SerializeObject(data);
        }

        internal async Task<string> GetPhotographyData()
        {
            MediaPhotographyData data = new MediaPhotographyData { Photos = new List<MediaPhoto>() };
            foreach (BsonDocument entry in await (await Database_Public_Data.GetCollection<BsonDocument>("photography-data").FindAsync(_ => true)).ToListAsync()) data.Photos.Add(BsonSerializer.Deserialize<MediaPhoto>(entry));
            return JsonConvert.SerializeObject(data);
        }
    }
}
