using System;

namespace UEESA.Client.Data
{
    public struct PageContext
    {
        public string FormalPageName { get; set; }
        public string InformalPageName { get; set; }
        public string PageBackgroundName { get; set; }
        public bool ForceNavBarTickersInvisible { get; set; }
    }
}
