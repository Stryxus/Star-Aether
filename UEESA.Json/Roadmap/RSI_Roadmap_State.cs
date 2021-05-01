using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UEESA.Json.Roadmap
{
    public class RSI_Roadmap_State
    {
        public Dictionary<string, Dictionary<RSI_Roadmap_State_Category, List<RSI_Roadmap_State_Feature>>> Features { get; set; }
        public Dictionary<string, RSI_Roadmap_State_Status> ReleaseStatus { get; set; }
    }
}
