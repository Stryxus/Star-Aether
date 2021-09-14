using System;
using System.Collections.Generic;

namespace UEESA.Data.Bson.Roadmap
{
    public class UEESA_Bson_Roadmap_Release
    {
        public string version { get; set; }
        public string release_date { get; set; }
        public DateTime creation_datetime { get; set; }
        public DateTime updated_datetime { get; set; }
        public bool has_released { get; set; }
        public List<UEESA_Bson_Roadmap_Card> cards { get; set; }
    }
}
