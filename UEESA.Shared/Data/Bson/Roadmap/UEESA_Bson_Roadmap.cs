using System;
using System.Collections.Generic;

namespace UEESA.Data.Bson.Roadmap
{
    public class UEESA_Bson_Roadmap
    {
        public DateTime UpdatedDateTime { get; set; }
        public List<UEESA_Bson_Roadmap_Release>? Releases { get; set; } = new();
    }
}
