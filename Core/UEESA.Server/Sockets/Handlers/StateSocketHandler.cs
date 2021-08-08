using System;
using System.Net.WebSockets;

using UEESA.Server.Data;
using UEESA.Shared.Data.Bson;
using UEESA.Shared.Sockets;

using Newtonsoft.Json;

namespace UEESA.Server.Sockets.Handlers
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
                if ((Commands)cmd == Commands.GetRoadmapData) SendMessageAsync(socket, "JSON." + typeof(RSI_Bson_Roadmap).Name + JsonConvert.SerializeObject(Services.Get<RSIRoadmapScraper>().Roadmap_Data));
            }

            return;
        }
    }
}
