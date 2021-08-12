
using UEESA.Client.Data.Json;
using UEESA.Shared.Data.Bson.Roadmap;

namespace UEESA.Client.Data.States
{
    internal class ClientState
    {
        // Global Variables

        internal Random Random = new();

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

        private UEESA_Bson_Roadmap roadmapData;
        internal UEESA_Bson_Roadmap RoadmapData
        {
            get
            {
                return roadmapData;
            }
            set
            {
                roadmapData = value;
                OnRoadmapDataUpdated?.Invoke();
                Services.Get<InitializationState>().CheckAppLoaded();
            }
        }

        internal event Action OnRoadmapDataUpdated;
        internal async void NotifyRoadmapCardDataChange(UEESA_Bson_Roadmap data, bool isLocalData)
        {
            if (!isLocalData)
            {
                RoadmapData = data;
                await Services.Get<LocalStorageState>().SetLocalData<UEESA_Bson_Roadmap>();
                Logger.LogInfo("Received Roadmap State.");
            }
        }
    }
}
