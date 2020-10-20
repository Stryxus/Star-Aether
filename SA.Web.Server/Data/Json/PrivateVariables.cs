using System;
using System.IO;

using Newtonsoft.Json;

namespace SA.Web.Server.Data.Json
{
    public class PrivateVariables
    {
        public string ApplicationInsightsKey { get; set; }
        public string MongoDBConnectionString { get; set; }

        // Admin controller

        public string Username { get; set; }
        public string Password { get; set; }
        public string AdminIP { get; set; }

        // Twitch

        public string TwitchAPIID { get; set; }

        // Discord

        public string Token { get; set; }
        public string GuildID { get; set; }
        public string DebugToken { get; set; }
        public string DebugChannelID { get; set; }

        //

        internal static PrivateVariables _instance;
        internal static PrivateVariables Instance 
        { 
            get
            {
                return _instance != null ? _instance : _instance = JsonConvert.DeserializeObject<PrivateVariables>(File.ReadAllText("private.json"));
            }
            private set
            {
                _instance = value;
            }
        }

        private static void GetJson() => Instance = JsonConvert.DeserializeObject<PrivateVariables>(File.ReadAllText("private.json"));

        internal static string GetAdminIP()
        {
            GetJson();
            return Instance.AdminIP;
        }
    }
}
