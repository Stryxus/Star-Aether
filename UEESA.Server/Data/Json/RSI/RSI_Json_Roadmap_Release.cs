<<<<<<< HEAD
<<<<<<< HEAD
﻿namespace UEESA.Server.Data.Json.RSI;
public class RSI_Json_Roadmap_Release
=======
﻿using System.Collections.Generic;

namespace UEESA.Server.Data.Json.RSI
>>>>>>> parent of 0f2b48d (Remove socket code and cleanup)
=======
﻿using System.Collections.Generic;

namespace UEESA.Server.Data.Json.RSI
>>>>>>> parent of 0f2b48d (Remove socket code and cleanup)
{
    public int Id { get; set; }
    public int time_created { get; set; }
    public int time_modified { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public int released { get; set; }
    public List<RSI_Json_Roadmap_Card> cards { get; set; }
}
