using System;
using System.Threading.Tasks;

using UEESA.Client.Data.Json;
using UEESA.Shared.Json;

using Newtonsoft.Json;
using UEESA.Client.WebSockets;

namespace UEESA.Client.Data.States
{
    public class LocalStorageState
    {
        public async Task SetLocalData<T>(bool defaultData = false)
        {
            switch (typeof(T))
            {
                case Type t when t == typeof(GlobalSettings):
                    await Services.Get<JSInterface.LocalData>().SetData(t.Name, JsonConvert.SerializeObject(Services.Get<ClientState>().Settings = defaultData ? new GlobalSettings() : Services.Get<ClientState>().Settings));
                    Services.Get<ClientState>().NotifySettingsChange();
                    break;
                case Type t when t == typeof(NewsData):
                    await Services.Get<JSInterface.LocalData>().SetData(t.Name, JsonConvert.SerializeObject(Services.Get<ClientState>().NewsData = defaultData ? new NewsData() : Services.Get<ClientState>().NewsData));
                    break;
                case Type t when t == typeof(RoadmapData):
                    await Services.Get<JSInterface.LocalData>().SetData(t.Name, JsonConvert.SerializeObject(Services.Get<ClientState>().RoadmapData = defaultData ? new RoadmapData() : Services.Get<ClientState>().RoadmapData));
                    break;
            }
        }

        public async Task GetLocalData<T>()
        {
            try
            {
                T obj;
                bool exists;
                switch (typeof(T))
                {
                    case Type t when t == typeof(GlobalSettings):
                        if (((exists, obj) = await LocalDataExists<T>()).exists)
                        {
                            Services.Get<ClientState>().Settings = obj as GlobalSettings ?? new GlobalSettings();
                            Services.Get<ClientState>().NotifySettingsChange();
                        }
                        else
                        {
                            Services.Get<ClientState>().Settings = new GlobalSettings();
                            await SetLocalData<T>();
                        }
                        break;
                    case Type t when t == typeof(NewsData):
                        if (((exists, obj) = await LocalDataExists<T>()).exists) Services.Get<ClientState>().NotifyNewsDataChange(Services.Get<ClientState>().NewsData = obj as NewsData ?? new NewsData(), true);
                        else if (Services.Get<WebSocketManagerMiddleware>().IsConnected) await Services.Get<ServerState>().RequestNewsData(true);
                        break;
                    case Type t when t == typeof(RoadmapData):
                        if (((exists, obj) = await LocalDataExists<T>()).exists) Services.Get<ClientState>().NotifyRoadmapCardDataChange(Services.Get<ClientState>().RoadmapData = obj as RoadmapData ?? new RoadmapData(), true);
                        else if (Services.Get<WebSocketManagerMiddleware>().IsConnected) await Services.Get<ServerState>().RequestRoadmapData(true);
                        break;
                }
            }
            catch (ArgumentNullException) { await SetLocalData<T>(true); }
        }

        public async Task<(bool, T?)> LocalDataExists<T>()
        {
            string content;
            if (string.IsNullOrEmpty(content = await Services.Get<JSInterface.LocalData>().GetData(typeof(T).Name)) || string.IsNullOrWhiteSpace(content)) return (false, default);
            else try { return (true, JsonConvert.DeserializeObject<T>(content)); } catch (JsonException) { return (false, default); }
        }
    }
}
