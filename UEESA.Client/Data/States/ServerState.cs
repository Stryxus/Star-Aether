using System.Net.WebSockets;

<<<<<<< HEAD
<<<<<<< HEAD
namespace UEESA.Client.Data.States;
internal class ServerState
=======
=======
>>>>>>> parent of 0f2b48d (Remove socket code and cleanup)
using UEESA.Client.Sockets;
using UEESA.Client.Sockets.Handlers;
using UEESA.Json.Client;
using UEESA.Json.External.RSI.Roadmap;

namespace UEESA.Client.Data.States
>>>>>>> parent of 0f2b48d (Remove socket code and cleanup)
{
    internal async Task RequestRoadmapData(bool getFreshCopy = false) => await Send(async (ClientWebSocket socket) =>
    {
<<<<<<< HEAD
        Logger.LogInfo("Requesting Roadmap Data...");
        // TODO
        //if (Services.Get<WebSocketManagerMiddleware>().IsConnected && getFreshCopy) Services.Get<StateSocketHandler>().SendMessageAsync(socket, new JClient_SocketDataCapsule<object> { Attributes = new List<string>() { "GetRoadmapData" } });
        //else await Services.Get<LocalStorageState>().GetLocalData<JRSI_Roadmap>();
    });

    private static Task Send(Action<ClientWebSocket> act)
    {
        // TODO
        //act.Invoke(Services.Get<WebSocketManagerMiddleware>().ClientSocket);
        return Task.CompletedTask;
=======
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
>>>>>>> parent of 0f2b48d (Remove socket code and cleanup)
    }
}
