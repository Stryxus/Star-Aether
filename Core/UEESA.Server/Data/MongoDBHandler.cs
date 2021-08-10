using System;
using System.Security.Authentication;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Bson;

using UEESA.Shared.Data.Bson.Roadmap;
using UEESA.Shared.Data.Bson.Users;

namespace UEESA.Server.Data
{
    public class MongoDBHandler
    {
        private MongoClient Client;
        private IMongoDatabase UEESA_DB;

        private IMongoCollection<UEESA_Bson_User> UEESA_USERS;
        private IMongoCollection<RSI_Bson_Roadmap> UEESA_ROADMAP_DATA;

        public MongoDBHandler()
        {
            Task.Run(() => 
            {
                Logger.LogInfo("Connecting to UEESA Database...");
#if DEBUG
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(PrivateData.Instance.DEV_MongoDBConnectionString));
#else
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(PrivateData.Instance.MongoDBConnectionString));
#endif
                settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
                Client = new(settings);
                UEESA_DB = Client.GetDatabase("ueesadb");
                UEESA_USERS = UEESA_DB.GetCollection<UEESA_Bson_User>("users");
                UEESA_ROADMAP_DATA = UEESA_DB.GetCollection<RSI_Bson_Roadmap>("roadmap_data");
                Logger.LogInfo("Connection to UEESA Database Successful!");
            });
        }
    }
}
