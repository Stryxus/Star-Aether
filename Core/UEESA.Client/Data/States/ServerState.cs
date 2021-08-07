using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

using UEESA.Client.Sockets;
using UEESA.Client.Sockets.Handlers;
using UEESA.Shared.Sockets;
using UEESA.Json.Roadmap;

namespace UEESA.Client.Data.States
{
    internal class ServerState
    {

        internal async Task RequestRoadmapData(bool getFreshCopy = false) => await Send(async (ClientWebSocket socket) =>
        {
            Logger.LogInfo("Requesting Roadmap Data...");
            if (Services.Get<WebSocketManagerMiddleware>().IsConnected && getFreshCopy)
                Services.Get<StateSocketHandler>().SendMessageAsync(socket, "CMD." + Commands.GetRoadmapData);
            else await Services.Get<LocalStorageState>().GetLocalData<RSI_Roadmap_State>();
        });

        private static Task Send(Action<ClientWebSocket> act)
        {
            act.Invoke(Services.Get<WebSocketManagerMiddleware>().ClientSocket);
            return Task.CompletedTask;
        }
    }
}
