using System;
using System.Linq;

using SA.Web.Shared.Json;
using SA.Web.Client.Data.Json;
using System.Collections.Generic;

namespace SA.Web.Client.Data.States
{
    internal class ClientState
    {
        // Global Variables

        internal Random Random = new Random();

        //

        internal event Action OnSettingsChanged;
        private GlobalSettings settings;
        internal GlobalSettings Settings 
        { 
            get 
            {
                return settings;
            }
            set
            {
                settings = value;
                OnSettingsChanged?.Invoke();
                Services.Get<InitializationState>().CheckAppLoaded();
            }
        }

        internal void NotifySettingsChange() => OnSettingsChanged?.Invoke();

        private NewsData _newsData;
        internal NewsData NewsData
        {
            get
            {
                return _newsData;
            }
            set
            {
                _newsData = value;
                OnNewsDataChanged?.Invoke();
                Services.Get<InitializationState>().CheckAppLoaded();
            }
        }

        internal event Action OnNewsDataChanged;
        internal async void NotifyNewsDataChange(NewsData data, bool isLocalData)
        {
            if (!isLocalData)
            {
                data.NewsPosts.Reverse();
                NewsData = data;
                await Services.Get<LocalStorageState>().SetLocalData<NewsData>();
                await Logger.LogInfo("Received News Data!");
            }
        }

        private RoadmapData _roadmapData;
        internal RoadmapData RoadmapData
        {
            get
            {
                return _roadmapData;
            }
            set
            {
                _roadmapData = value;
                OnRoadmapCardDataChanged?.Invoke();
                Services.Get<InitializationState>().CheckAppLoaded();
            }
        }

        internal event Action OnRoadmapCardDataChanged;
        internal async void NotifyRoadmapCardDataChange(RoadmapData data, bool isLocalData)
        {
            data.Cards = data.Cards.OrderBy(o => o.MajorVersion).ThenBy(o => o.MinorVersion).Reverse().ToList();
            for (int i = 0; i < data.Cards.Count - 1; i++) data.Cards[i].Patches = data.Cards[i].Patches.OrderBy(o => o.PatchVersion).ToList();
            if (!isLocalData)
            {
                RoadmapData = data;
                await Services.Get<LocalStorageState>().SetLocalData<RoadmapData>();
                await Logger.LogInfo("Received Roadmap Data!");
            }
        }

        // Twitch

        /*
         * TODO:
         * 
         * Add in Person Profile class to simplify fetching entire profiles of people who are featured. Could bring over official twitch, youtube, twitter and more data with this. 
         * 
         */

        internal event Action<string> OnTwitchLogoAdded;
        internal Dictionary<string, string> TwitchLogos { get; private set; } = new Dictionary<string, string>();

        internal void AddTwitchLogo(string username, string url)
        {
            if (!TwitchLogos.ContainsKey(username))
            {
                TwitchLogos.Add(username, url);
                OnTwitchLogoAdded?.Invoke(username);
            }
        }
    }
}
