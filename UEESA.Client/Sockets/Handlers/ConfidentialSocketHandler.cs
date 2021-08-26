﻿using System.Net.WebSockets;

using UEESA.Sockets;

using Newtonsoft.Json.Linq;

namespace UEESA.Client.Sockets.Handlers
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