﻿using System;
using System.Collections.Generic;

namespace UEESA.Shared.Data.Bson.Roadmap
{
    public class UEESA_Bson_Roadmap
    {
        public DateTime updated_datetime { get; set; }
        public List<UEESA_Bson_Roadmap_Release> releases { get; set; }
    }
}