using System;
using System.Collections.Generic;

namespace UEESA.Data.Bson.Roadmap
{
    public class UEESA_Bson_Roadmap_Release
    {
        public string? Version { get; set; }
        public string? ReleaseDate { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public bool HasReleased { get; set; }
        public List<UEESA_Bson_Roadmap_Card>? Cards { get; set; } = new();
    }
}
