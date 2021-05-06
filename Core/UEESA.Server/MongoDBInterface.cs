using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Diagnostics;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using UEESA.Server.WebSockets;
using UEESA.Json.Roadmap;
using UEESA.RSIScraper.Roadmap;

using Stryxus.Lib.FileSystem;

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
#if DEBUG
                SetupDebugMongoDB().GetAwaiter().GetResult();
#endif

                Logger.LogInfo("Connecting to MongoDB...");
#if DEBUG
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl("mongodb://localhost:27017"));
#else
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(PrivateData.Instance.MongoDBConnectionString));
#endif
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

#if DEBUG
        private static async Task SetupDebugMongoDB()
        {
            DirectoryInfo baseDir = new(Path.Combine(FileSystemHelper.ApplicationDirectory.FullName, "MongoDB"));
            DirectoryInfo DBDir = new(Path.Combine(baseDir.FullName, "DB"));
            FileInfo mongodexe = new(Path.Combine(baseDir.FullName, "mongod.exe"));
            FileInfo mongoDBZip = new(Path.Combine(baseDir.FullName, "mongodb.zip"));
            if (!baseDir.Exists) await FileSystemHelper.Create(baseDir);
            if (!DBDir.Exists) await FileSystemHelper.Create(DBDir);
            if (!mongodexe.Exists)
            {
                Logger.LogWarn("Downloaiding MongoDB Community Server ZIP to " + baseDir.FullName + " for testing...");
                
                using WebClient wc = new(); await wc.DownloadFileTaskAsync(new Uri("https://fastdl.mongodb.org/windows/mongodb-windows-x86_64-4.4.5.zip"), Path.Combine(baseDir.FullName, "mongodb.zip"));
                using (ZipArchive zipArchive = ZipFile.Open(mongoDBZip.FullName, ZipArchiveMode.Read))
                {
                    List<ZipArchiveEntry> executables = zipArchive.Entries.Where(x => x.FullName.ToLower().EndsWith(mongodexe.Name + "." + mongodexe.Extension)).ToList();
                    foreach (ZipArchiveEntry entry in executables)
                    {
                        using Stream stream = entry.Open();
                        using FileStream file = await FileIOHelper.OpenStream(new FileInfo(Path.Combine(baseDir.FullName, entry.FullName[(entry.FullName.LastIndexOf(Path.AltDirectorySeparatorChar) + 1)..])), FileMode.OpenOrCreate, FileAccess.Write);
                        int current;
                        while ((current = stream.ReadByte()) != -1) file.WriteByte((byte)current);
                    }
                }
                await FileSystemHelper.Delete(mongoDBZip);
                StartMongod();
            }
            else StartMongod();

            void StartMongod() => Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(baseDir.FullName, "mongod.exe"),
                Arguments = "--dbpath " + '"' + Path.Combine(baseDir.FullName, "DB") + '"'
            });
        }
#endif
    }
}
