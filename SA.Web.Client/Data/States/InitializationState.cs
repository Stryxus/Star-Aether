using System;
using System.Threading.Tasks;

using SA.Web.Client.WebSockets;
using SA.Web.Client.Data.Json;

namespace SA.Web.Client.Data.States
{
    internal class InitializationState
    {
        internal event Action OnAppLoaded;
        internal void CheckAppLoaded()
        {
            if (
                Services.Get<ClientState>().Settings != null &&
                Services.Get<ClientState>().LocalUpdateTimes != null &&
                Services.Get<ClientState>().NewsData != null &&
                Services.Get<ClientState>().ChangelogData != null &&
                Services.Get<ClientState>().RoadmapData != null &&
                Services.Get<ClientState>().PhotographyData != null
                ) OnAppLoaded?.Invoke();
        }

        private bool FirstRegisterPass { get; set; } = true;
        private bool FirstDataLoadPass { get; set; } = true;

        internal async Task Init()
        {
            if (FirstRegisterPass)
            {
                FirstRegisterPass = false;

                await Logger.LogInfo("Initializing Client State...");

                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.Runtime>(), "runtime");
                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.Cache>(), "cacheStorageInterface");
                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.LocalData>(), "localStorageInterface");
                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.WebGLInterface>(), "webGLInterface");
                await Services.Get<JSInterface>().InitializeInterface(Services.Get<JSInterface.AnimationManager>(), "animationInterface");

                OnAppLoaded += async () => await Services.Get<JSInterface.AnimationManager>().ShowRouter();

                await Services.Get<LocalStorageState>().GetLocalData<GlobalSettings>();

                Services.Get<WebSocketManagerMiddleware>().OnServerConnected += async () =>
                {
                    Services.Get<ClientState>().OnUpdateTimesChanged += async () =>
                    {
                        if (FirstDataLoadPass)
                        {
                            FirstDataLoadPass = false;
                            if (Services.Get<ClientState>().PreviousLocalUpdateTimes != null && Services.Get<ClientState>().PreviousLocalUpdateTimes != Services.Get<ClientState>().LocalUpdateTimes)
                            {
                                if (DateTime.Compare(Services.Get<ClientState>().PreviousLocalUpdateTimes.ChangelogDataUpdate, Services.Get<ClientState>().LocalUpdateTimes.ChangelogDataUpdate) < 0) await Services.Get<ServerState>().RequestChangelogData(true);
                                if (DateTime.Compare(Services.Get<ClientState>().PreviousLocalUpdateTimes.NewsDataUpdate, Services.Get<ClientState>().LocalUpdateTimes.NewsDataUpdate) < 0) await Services.Get<ServerState>().RequestNewsData(true);
                                if (DateTime.Compare(Services.Get<ClientState>().PreviousLocalUpdateTimes.RoadmapDataUpdate, Services.Get<ClientState>().LocalUpdateTimes.RoadmapDataUpdate) < 0) await Services.Get<ServerState>().RequestRoadmapData(true);
                                if (DateTime.Compare(Services.Get<ClientState>().PreviousLocalUpdateTimes.PhotographyDataUpdate, Services.Get<ClientState>().LocalUpdateTimes.PhotographyDataUpdate) < 0) await Services.Get<ServerState>().RequestPhotographyData(true);
                            }
                            else if (Services.Get<ClientState>().PreviousLocalUpdateTimes == Services.Get<ClientState>().LocalUpdateTimes)
                            {
                                await Services.Get<ServerState>().RequestChangelogData(true);
                                await Services.Get<ServerState>().RequestNewsData(true);
                                await Services.Get<ServerState>().RequestRoadmapData(true);
                                await Services.Get<ServerState>().RequestPhotographyData(true);
                            }
                            else
                            {
                                await Services.Get<ServerState>().RequestChangelogData();
                                await Services.Get<ServerState>().RequestNewsData();
                                await Services.Get<ServerState>().RequestRoadmapData();
                                await Services.Get<ServerState>().RequestPhotographyData();
                            }
                        }
                    };
                    await Services.Get<ServerState>().RequestUpdateData(true);
                };
                Services.Get<WebSocketManagerMiddleware>().OnServerConnectionError += async () =>
                {
                    await Logger.LogInfo("Connection to the server cannot be established. Running in offline mode.");
                    await Services.Get<ServerState>().RequestChangelogData();
                    await Services.Get<ServerState>().RequestNewsData();
                    await Services.Get<ServerState>().RequestRoadmapData();
                    await Services.Get<ServerState>().RequestPhotographyData();
                };
                await Services.Get<WebSocketManagerMiddleware>().Connect(Services.Get<ClientState>());

                await Logger.LogInfo("Client State Initialized.");
            }
        }
    }
}
