using System;
using System.Collections.Generic;

namespace UEESA.Server.Data.Json.RSI
{
    public class RSI_Json_Roadmap_Card
    {
        public int time_created { get; set; }
        public int time_modified { get; set; }
        public string name { get; set; }
        public int category_id { get; set; }
        public string body { get; set; }
        public int order { get; set; }
        public string status { get; set; }
        public int released { get; set; }
    }
}
