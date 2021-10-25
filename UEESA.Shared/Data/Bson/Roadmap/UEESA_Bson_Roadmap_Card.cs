using System;
using System.Collections.Generic;

namespace UEESA.Data.Bson.Roadmap
{
    public class UEESA_Bson_Roadmap_Card
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public UEESA_Bson_Roadmap_Card_Category Category { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string? ThumnailURL { get; set; }
        public RSI_Bson_Roadmap_Card_Status Status { get; set; }
        public bool HasReleased { get; set; }
        public List<string>? Teams { get; set; } = new();
    }
}
