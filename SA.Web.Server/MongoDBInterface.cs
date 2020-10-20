using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using SA.Web.Server.Data.Json;
using SA.Web.Server.WebSockets;
using SA.Web.Shared.Json;

using Newtonsoft.Json;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace SA.Web.Server
{
    internal class MongoDBInterface
    {
        private MongoClient Client;
        private IMongoDatabase Database_Public_Data;
        private IMongoCollection<BsonDocument> Bson_UpdateTimes;
        private IMongoCollection<BsonDocument> Bson_NewsData;
        private IMongoCollection<BsonDocument> Bson_RoadmapData;
        private IMongoCollection<BsonDocument> Bson_ChangelogData;
        private IMongoCollection<BsonDocument> Bson_FeaturedData;
        private IMongoCollection<BsonDocument> Bson_PhotographyData;

        internal event Action OnPublicDataUpdate;
        internal void InvokeOnPublicDataUpdate() => OnPublicDataUpdate?.Invoke();

        private LastUpdateTimes UpTimes;

        internal async Task Connect()
        {
            if (Client == null)
            {
                await Logger.LogInfo("Connecting to MongoDB...");
                Client = new MongoClient(PrivateVariables.Instance.MongoDBConnectionString);
                await Client.StartSessionAsync();
                Database_Public_Data = Client.GetDatabase("public_data");
                Bson_UpdateTimes = Database_Public_Data.GetCollection<BsonDocument>("update_times_data");
                Bson_NewsData = Database_Public_Data.GetCollection<BsonDocument>("news_data");
                Bson_RoadmapData = Database_Public_Data.GetCollection<BsonDocument>("roadmap_data");
                Bson_ChangelogData = Database_Public_Data.GetCollection<BsonDocument>("changelog_data");
                Bson_PhotographyData = Database_Public_Data.GetCollection<BsonDocument>("photography_data");
                await Logger.LogInfo("Connected to MongoDB!");

                OnPublicDataUpdate += async () =>
                {
                    LastUpdateTimes latestTimes = JsonConvert.DeserializeObject<LastUpdateTimes>(GetUpdateTimesData());
                    Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(LastUpdateTimes).Name + Bson_UpdateTimes.ToJson());
                    if (UpTimes == null || latestTimes.NewsDataUpdate != UpTimes.NewsDataUpdate) Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(NewsData).Name + await GetNewsData());
                    else if (UpTimes == null || latestTimes.RoadmapDataUpdate != UpTimes.RoadmapDataUpdate) Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(RoadmapData).Name + await GetRoadmapData());
                    else if (UpTimes == null || latestTimes.ChangelogDataUpdate != UpTimes.ChangelogDataUpdate) Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(ChangelogData).Name + await GetChangelogData());
                    else if (UpTimes == null || latestTimes.PhotographyDataUpdate != UpTimes.PhotographyDataUpdate) Services.Get<StateSocketHandler>().SendMessageToAllAsync("JSON." + typeof(MediaPhotographyData).Name + await GetPhotographyData());
                    UpTimes = latestTimes;
                };
            }
        }

        internal string GetUpdateTimesData() => Bson_UpdateTimes.ToJson();

        internal async Task<string> GetNewsData()
        {
            NewsData data = new NewsData { NewsPosts = new List<NewsEntryData>() };
            foreach (BsonDocument entry in await (await Bson_NewsData.FindAsync(_ => true)).ToListAsync()) data.NewsPosts.Add(BsonSerializer.Deserialize<NewsEntryData>(entry));
            return JsonConvert.SerializeObject(data);
        }

        internal async Task<string> GetRoadmapData()
        {
            RoadmapData data = new RoadmapData { Cards = new List<RoadmapCard>() };
            foreach (BsonDocument entry in await (await Bson_RoadmapData.FindAsync(_ => true)).ToListAsync()) data.Cards.Add(BsonSerializer.Deserialize<RoadmapCard>(entry));
            return JsonConvert.SerializeObject(data);
        }

        internal async Task<string> GetChangelogData()
        {
            ChangelogData data = new ChangelogData { ChangelogPosts = new List<ChangelogEntryData>() };
            foreach (BsonDocument entry in await (await Bson_ChangelogData.FindAsync(_ => true)).ToListAsync()) data.ChangelogPosts.Add(BsonSerializer.Deserialize<ChangelogEntryData>(entry));
            return JsonConvert.SerializeObject(data);
        }

        internal async Task<string> GetPhotographyData()
        {
            MediaPhotographyData data = new MediaPhotographyData { Photos = new List<MediaPhoto>() };
            foreach (BsonDocument entry in await (await Bson_PhotographyData.FindAsync(_ => true)).ToListAsync()) data.Photos.Add(BsonSerializer.Deserialize<MediaPhoto>(entry));
            return JsonConvert.SerializeObject(data);
        }
    }
}
