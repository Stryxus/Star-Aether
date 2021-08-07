using System;

using UEESA.Client.Data.States;

namespace UEESA.Client.Data.Json
{
    public class GlobalSettings
    {
        // User Interface
        public bool ShowHealinesTicker = true;
        public bool ShowEconomeTicker = true;

        // Push notifications
        public bool ShowBlogUpdateNotification { get; set; } = true;
        public bool ShowChangelogUpdateNotification { get; set; } = true;
        public bool ShowRoadmapUpdateNotification { get; set; } = true;
        public bool ShowVideographyUpdateNotification { get; set; } = true;
    }
}
