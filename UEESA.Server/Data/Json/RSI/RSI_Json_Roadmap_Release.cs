using System.Collections.Generic;

namespace UEESA.Server.Data.Json.RSI
{
    public class RSI_Json_Roadmap_Release
    {
        public int Id { get; set; }
        public int time_created { get; set; }
        public int time_modified { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int released { get; set; }
        public List<RSI_Json_Roadmap_Card> cards { get; set; }
    }
}
