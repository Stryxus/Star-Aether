using System.Net.WebSockets;

using UEESA.Client.Sockets;
using UEESA.Client.Sockets.Handlers;
using UEESA.Json.Client;
using UEESA.Json.External.RSI.Roadmap;

namespace UEESA.Client.Data.States
{
    internal class ServerState
    {
        internal async Task RequestRoadmapData(bool getFreshCopy = false) => await Send(async (ClientWebSocket socket) =>
        {
            Logger.LogInfo("Requesting Roadmap Data...");
            if (Services.Get<WebSocketManagerMiddleware>().IsConnected && getFreshCopy)
                Services.Get<StateSocketHandler>().SendMessageAsync(socket, new JClient_SocketDataCapsule<object> { Attributes = new List<string>() { "GetRoadmapData" } });
            else await Services.Get<LocalStorageState>().GetLocalData<JRSI_Roadmap>();
        });

        private static Task Send(Action<ClientWebSocket> act)
        {
            act.Invoke(Services.Get<WebSocketManagerMiddleware>().ClientSocket);
            return Task.CompletedTask;
        }
    }
}
