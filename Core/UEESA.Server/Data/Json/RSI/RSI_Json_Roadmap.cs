using System;
using System.Collections.Generic;

namespace UEESA.Server.Data.Json.RSI
{
    public class RSI_Json_Roadmap
    {
        public string description { get; set; }
        public string notification_body { get; set; }
        public List<RSI_Json_Roadmap_Release> releases { get; set; }
        public int last_updated { get; set; }
    }
}
