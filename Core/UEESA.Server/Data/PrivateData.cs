
using Newtonsoft.Json;

public class PrivateData
{
    // Production

    public string MongoDBConnectionString { get; set; }
    public string ApplicationInsightsConnectionString { get; set; }
    public string MicrosoftIdentityPlatformClientID { get; set; }

    // Development

    public string DEV_MongoDBConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DEV_ApplicationInsightsConnectionString { get; set; } = "00000000-0000-0000-0000-000000000000";
    public string DEV_MicrosoftIdentityPlatformClientID { get; set; } = string.Empty;

    //

    internal static PrivateData _instance;
    internal static PrivateData Instance
    {
        get { return _instance != null ? _instance : _instance = JsonConvert.DeserializeObject<PrivateData>(File.ReadAllText("private.json")); }
        private set { _instance = value; }
    }
}