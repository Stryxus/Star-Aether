using System;
using System.IO;

using Newtonsoft.Json;

namespace SA.Web.Server.Data
{
    public class PrivateVariables
    {
        public string MongoDBConnectionString { get; set; }

        //

        internal static PrivateVariables _instance;
        internal static PrivateVariables Instance
        {
            get { return _instance != null ? _instance : _instance = JsonConvert.DeserializeObject<PrivateVariables>(File.ReadAllText("private.json")); }
            private set { _instance = value; }
        }
    }
}