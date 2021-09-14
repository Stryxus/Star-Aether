using System;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Collections.Generic;

using UEESA.Client.Sockets;
using UEESA.Client.Sockets.Handlers;
using UEESA.Data.Bson.Roadmap;
using UEESA.Data.Json;

namespace UEESA.Client.Data.States
{
    internal class ServerState
    {
        internal async Task RequestRoadmapData(bool getFreshCopy = false) => await Send(async (ClientWebSocket socket) =>
        {
            Logger.LogInfo("Requesting Roadmap Data...");
            if (Services.Get<WebSocketManagerMiddleware>().IsConnected && getFreshCopy)
                Services.Get<StateSocketHandler>().SendMessageAsync(socket, new UEESA_Json_StateSocketDataCapsule<object> { attributes = new List<string>() { "GetRoadmapData" } });
            else await Services.Get<LocalStorageState>().GetLocalData<UEESA_Bson_Roadmap>();
        });

        private static Task Send(Action<ClientWebSocket> act)
        {
            act.Invoke(Services.Get<WebSocketManagerMiddleware>().ClientSocket);
            return Task.CompletedTask;
        }
    }
}
