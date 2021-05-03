using System;

using UEESA.Client.Data.Json;
using UEESA.Json.Roadmap;

namespace UEESA.Client.Data.States
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

        private RSI_Roadmap_State _roadmapState;
        internal RSI_Roadmap_State RoadmapState
        {
            get
            {
                return _roadmapState;
            }
            set
            {
                _roadmapState = value;
                OnRoadmapStateChanged?.Invoke();
                Services.Get<InitializationState>().CheckAppLoaded();
            }
        }

        internal event Action OnRoadmapStateChanged;
        internal async void NotifyRoadmapCardDataChange(RSI_Roadmap_State data, bool isLocalData)
        {
            if (!isLocalData)
            {
                RoadmapState = data;
                await Services.Get<LocalStorageState>().SetLocalData<RSI_Roadmap_State>();
                Logger.LogInfo("Received Roadmap State.");
            }
        }
    }
}
