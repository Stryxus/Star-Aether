using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UEESA.Json.Roadmap
{
    public class RSI_Roadmap_State_Feature
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public RSI_Roadmap_State_Status Status { get; set; }
        public List<string> Teams { get; set; }
        public string ThumbnailLink { get; set; }
    }
}
