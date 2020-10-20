using System;

namespace SA.Web.Client.Data.Json
{
    public class GlobalSettings
    {
        // User Interface
        public bool ShowHealinesTicker { get; set; } = true;
        public bool ShowEconomyTicker { get; set; } = true;

        // Push notifications
        public bool ShowBlogUpdateNotification { get; set; } = true;
        public bool ShowChangelogUpdateNotification { get; set; } = true;
        public bool ShowRoadmapUpdateNotification { get; set; } = true;
        public bool ShowPhotographyUpdateNotification { get; set; } = true;
        public bool ShowVideographyUpdateNotification { get; set; } = true;
    }
}
