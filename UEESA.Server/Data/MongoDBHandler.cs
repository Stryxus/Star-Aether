using System.Security.Authentication;
using UEESA.Json.External.RSI.Roadmap;

using MongoDB.Driver;

namespace UEESA.Server.Data
{
    public class MongoDBHandler
    {
        private MongoClient Client;
        private IMongoDatabase UEESA_DB;

        //private IMongoCollection<UEESA_Bson_User> UEESA_USERS;
        private IMongoCollection<JRSI_Roadmap> UEESA_BLOG_DATA;
        private IMongoCollection<JRSI_Roadmap> UEESA_ChANGELOG_DATA;
        private IMongoCollection<JRSI_Roadmap> UEESA_ROADMAP_DATA;

        public MongoDBHandler()
        {
            Task.Run(() =>
            {
                Logger.LogInfo("Connecting to UEESA Database...");
                MongoClientSettings settings;
                try
                {
                    settings = MongoClientSettings.FromUrl(new MongoUrl(Services.Configuration["COSMOSDB_CONNECTIONSTRING"]));
                }
                catch
                {
                    try
                    {
                        settings = MongoClientSettings.FromUrl(new MongoUrl(Services.Configuration["DEV_COSMOSDB_CONNECTIONSTRING"]));
                    }
                    catch
                    {
                        settings = MongoClientSettings.FromUrl(new MongoUrl("mongodb://localhost:27017"));
                    }
                }
                settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
                Client = new(settings);
                UEESA_DB = Client.GetDatabase("ueesadb");
                //UEESA_USERS = UEESA_DB.GetCollection<UEESA_Bson_User>("users");
                UEESA_BLOG_DATA = UEESA_DB.GetCollection<JRSI_Roadmap>("blog_data");
                UEESA_ChANGELOG_DATA = UEESA_DB.GetCollection<JRSI_Roadmap>("changelog_data");
                UEESA_ROADMAP_DATA = UEESA_DB.GetCollection<JRSI_Roadmap>("roadmap_data");
                Logger.LogInfo("Connected to UEESA Database!");
            });
        }

        // Blog

        internal async Task StoreBlogRecord()
        {

        }

        internal async Task PullBlogRecord()
        {

        }

        internal async Task PullAllBlogRecords()
        {

        }

        internal async Task DropBlogRecord()
        {

        }

        // Changelog

        internal async Task StoreChangelogRecord()
        {

        }

        internal async Task PullChangelogRecord()
        {

        }

        internal async Task PullAllChangelogRecords()
        {

        }

        internal async Task DropChangelogRecord()
        {

        }

        // Roadmap

        internal async Task StoreHistoracleRoadmapRecord()
        {

        }

        internal async Task PullHistoracleRoadmapRecord()
        {

        }

        internal async Task PullAllHistoracleRoadmapRecords()
        {

        }

        internal async Task DropHistoracleRoadmapRecord()
        {

        }
    }
}
