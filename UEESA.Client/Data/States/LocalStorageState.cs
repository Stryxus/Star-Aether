using UEESA.Client.Data.Json;

using Newtonsoft.Json;

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
                        }
                        else
                        {
                            Services.Get<ClientState>().Settings = new GlobalSettings();
                            await SetLocalData<T>();
                        }
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
