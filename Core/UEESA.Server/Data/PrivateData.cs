using System;
using System.IO;

using Newtonsoft.Json;

public class PrivateData
{
    public string MongoDBConnectionString { get; set; }
    public string ApplicationInsightsConnectionString { get; set; }
    public string MicrosoftIdentityPlatformClientID { get; set; }
    public string GoogleIdentityPlatformClientID { get; set; }
    public string GoogleIdentityPlatformClientSecret { get; set; }

    //

    internal static PrivateData _instance;
    internal static PrivateData Instance
    {
        get { return _instance != null ? _instance : _instance = JsonConvert.DeserializeObject<PrivateData>(File.ReadAllText("private.json")); }
        private set { _instance = value; }
    }
}