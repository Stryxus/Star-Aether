using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

using SA.Web.Client.WebSockets;
using SA.Web.Shared.Json;

namespace SA.Web.Client.Data.States
{
    internal class ServerState
    {
        internal async Task RequestNewsData(bool getFreshCopy = false) => await Send(async (ClientWebSocket socket) =>
        {
            await Logger.LogInfo("Requesting News Data...");
            if (Services.Get<WebSocketManagerMiddleware>().IsConnected && getFreshCopy)
                Services.Get<StateSocketHandler>().SendMessageAsync(socket, "CMD." + Commands.GetNewsData);
            else await Services.Get<LocalStorageState>().GetLocalData<NewsData>();
        });

        internal async Task RequestChangelogData(bool getFreshCopy = false) => await Send(async (ClientWebSocket socket) =>
        {
            await Logger.LogInfo("Requesting Changelog Data...");
            if (Services.Get<WebSocketManagerMiddleware>().IsConnected && getFreshCopy)
                Services.Get<StateSocketHandler>().SendMessageAsync(socket, "CMD." + Commands.GetChangelogData);
            else await Services.Get<LocalStorageState>().GetLocalData<ChangelogData>();
        });

        internal async Task RequestRoadmapData(bool getFreshCopy = false) => await Send(async (ClientWebSocket socket) =>
        {
            await Logger.LogInfo("Requesting Roadmap Data...");
            if (Services.Get<WebSocketManagerMiddleware>().IsConnected && getFreshCopy)
                Services.Get<StateSocketHandler>().SendMessageAsync(socket, "CMD." + Commands.GetRoadmapData);
            else await Services.Get<LocalStorageState>().GetLocalData<RoadmapData>();
        });

        internal async Task RequestTwitchLogo(string username) => await Send(async (ClientWebSocket socket) => 
        {
            await Logger.LogInfo("Requesting Twitch Data for " + username);
            if (Services.Get<WebSocketManagerMiddleware>().IsConnected) Services.Get<StateSocketHandler>().SendMessageAsync(socket, "TWITCH_LOGO." + username);
        });

        private static Task Send(Action<ClientWebSocket> act)
        {
            act.Invoke(Services.Get<WebSocketManagerMiddleware>().ClientSocket);
            return Task.CompletedTask;
        }
    }
}
