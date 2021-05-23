using System;
using System.Threading.Tasks;

using UEESA.Client.WebSockets;
using UEESA.Client.Data.Json;

namespace UEESA.Client.Data.States
{
    internal class InitializationState
    {
        internal event Action OnAppLoaded;
        private bool SingleAppLoadedLock;
        internal void CheckAppLoaded()
        {
            if (
                Services.Get<ClientState>().Settings != null &&
                !Services.Get<UIState>().FirstRender &&
                !SingleAppLoadedLock
                )
            {
                SingleAppLoadedLock = true;
                OnAppLoaded?.Invoke();
            }
        }

        private bool FirstRegisterPass { get; set; } = true;
        private bool FirstDataLoadPass { get; set; } = true;

        internal async Task Init()
        {
            if (FirstRegisterPass)
            {
                FirstRegisterPass = false;

                Logger.LogInfo("Initializing Client State...");

                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.Runtime>(), "runtime");
                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.Cache>(), "cacheStorageInterface");
                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.LocalData>(), "localStorageInterface");
                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.AnimationManager>(), "animationInterface");

                await Services.Get<LocalStorageState>().GetLocalData<GlobalSettings>();

                Services.Get<WebSocketManagerMiddleware>().OnServerConnected += async () =>
                {
                    if (FirstDataLoadPass)
                    {
                        FirstDataLoadPass = false;
                        await Services.Get<ServerState>().RequestRoadmapData(true);
                    }
                };
                Services.Get<WebSocketManagerMiddleware>().OnServerConnectionError += async () =>
                {
                    Logger.LogInfo("Connection to the server cannot be established. Running in offline mode.");
                    await Services.Get<ServerState>().RequestRoadmapData();
                };
                await Services.Get<WebSocketManagerMiddleware>().Connect(Services.Get<ClientState>());

                Logger.LogInfo("Client State Initialized.");
            }
        }
    }
}
