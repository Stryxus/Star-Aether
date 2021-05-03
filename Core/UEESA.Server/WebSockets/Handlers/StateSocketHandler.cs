using System;
using System.Net.WebSockets;

using UEESA.Shared.WebSockets;
using UEESA.Json.Roadmap;

namespace UEESA.Server.WebSockets
{
    public class StateSocketHandler : WebSocketHandler<WebSocket>
    {
        public StateSocketHandler(ConnectionManager<WebSocket> webSocketConnectionManager) : base(webSocketConnectionManager) { }

        public override void OnConnected(WebSocket socket)  => base.OnConnected(socket);

        public override void Receive(WebSocket socket, WebSocketReceiveResult result, string message)
        {
            message = message.Replace("\0", string.Empty);

            if (message.StartsWith("CMD.") && Enum.TryParse(typeof(Commands), message.Replace("CMD.", string.Empty), out object cmd))
            {
                if ((Commands)cmd == Commands.GetRoadmapData) SendMessageAsync(socket, "JSON." + typeof(RSI_Roadmap_State).Name + Services.Get<MongoDBInterface>().GetRoadmapData());
            }

            return;
        }
    }
}
