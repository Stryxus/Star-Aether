using System;
using System.Collections.Generic;

namespace UEESA.Shared.Data.Bson.Roadmap
{
    public class RSI_Bson_Roadmap
    {
        public DateTime updated_datetime { get; set; }
        public List<RSI_Bson_Roadmap_Release> releases { get; set; }
    }
}
