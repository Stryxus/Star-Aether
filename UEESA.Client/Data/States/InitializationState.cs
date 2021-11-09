using UEESA.Client.Data.Json;

using Microsoft.JSInterop;

namespace UEESA.Client.Data.States
{
    internal class InitializationState
    {
        internal event Action OnAppLoaded;
        private bool SingleAppLoadedLock;
        internal void CheckAppLoaded()
        {
            if (Services.Get<ClientState>().Settings != null && !SingleAppLoadedLock)
            {
                SingleAppLoadedLock = true;
                OnAppLoaded?.Invoke();
            }
        }

        private bool FirstRegisterPass { get; set; } = true;
        private bool FirstDataLoadPass { get; set; } = true;

        internal async Task Init(IJSRuntime runtime)
        {
            if (FirstRegisterPass)
            {
                FirstRegisterPass = false;

                Logger.LogInfo("Initializing Client State...");

                Services.Get<JSInterface>().SetJSRuntime(runtime);
                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.Runtime>(), "runtime");
                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.Cache>(), "cacheStorageInterface");
                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.LocalData>(), "localStorageInterface");
                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.AnimationManager>(), "animationInterface");

                await Services.Get<LocalStorageState>().GetLocalData<GlobalSettings>();

                /*
                 * TODO
                Services.Get<WebSocketManagerMiddleware>().OnServerConnected += async () =>
                {
                    if (FirstDataLoadPass)
                    {
                        FirstDataLoadPass = false;
                        await Services.Get<ServerState>().RequestRoadmapData(true);
                    }
                };
                await Services.Get<WebSocketManagerMiddleware>().Connect();
                */

                Logger.LogInfo("Client State Initialized.");
            }
        }
    }
}
