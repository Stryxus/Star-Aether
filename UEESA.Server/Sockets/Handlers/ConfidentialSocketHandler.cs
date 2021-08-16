using System.Net.WebSockets;

using Newtonsoft.Json.Linq;

using UEESA.Sockets;

namespace UEESA.Server.Sockets.Handlers
{
    public class ConfidentialSocketHandler : WebSocketHandler<WebSocket>
    {
        public ConfidentialSocketHandler(ConnectionManager<WebSocket> webSocketConnectionManager) : base(webSocketConnectionManager) { }

        public override void OnConnected(WebSocket socket) => base.OnConnected(socket);

        public override void Receive(WebSocket socket, WebSocketReceiveResult result, JObject message)
        {

        }
    }
}
