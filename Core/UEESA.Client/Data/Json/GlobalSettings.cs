using System;

using UEESA.Client.Data.States;

namespace UEESA.Client.Data.Json
{
    public class GlobalSettings
    {
        // User Interface
        private bool showheadlinesTicker = true;
        public bool ShowHealinesTicker 
        {
            get 
            {
                return showheadlinesTicker;
            }
            set
            {
                showheadlinesTicker = value;
                Services.Get<UIState>().IsHeadlinesNavBarTickerVisible = value;
            }
        }
        private bool showEconomeTicker = true;
        public bool ShowEconomeTicker
        {
            get
            {
                return showEconomeTicker;
            }
            set
            {
                showEconomeTicker = value;
                Services.Get<UIState>().IsEconomeNavBarTickerVisible = value;
            }
        }

        // Push notifications
        public bool ShowBlogUpdateNotification { get; set; } = true;
        public bool ShowChangelogUpdateNotification { get; set; } = true;
        public bool ShowRoadmapUpdateNotification { get; set; } = true;
        public bool ShowVideographyUpdateNotification { get; set; } = true;
    }
}
