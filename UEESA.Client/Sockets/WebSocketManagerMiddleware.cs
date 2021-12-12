﻿using System.Net.WebSockets;
using System.Text;

using UEESA.Client.Sockets.Handlers;
using UEESA.Sockets;

using Newtonsoft.Json.Linq;

namespace UEESA.Client.Sockets
{
    internal class WebSocketManagerMiddleware
    {
        public SocketHandler<ClientWebSocket> SocketHandler { get; private set; }
        public ClientWebSocket ClientSocket { get; private set; }
        public bool IsConnected { get; private set; }
        public event Action OnServerConnected;

        internal WebSocketManagerMiddleware(StateSocketHandler handler) { SocketHandler = handler; }

        internal async Task Connect()
        {
            if (ClientSocket != null) return;
            else
            {
                ClientSocket = new();
#if DEBUG
                await ClientSocket.ConnectAsync(new Uri("wss://localhost:5001/state"), CancellationToken.None).ContinueWith(async (task) => await Continued());
#else
                await ClientSocket.ConnectAsync(new Uri("wss://staraether.com/state"), CancellationToken.None).ContinueWith(async (task) => await Continued());
#endif

                async Task Continued()
                {
                    if (ClientSocket.State == WebSocketState.Open)
                    {
                        SocketHandler.OnConnected(ClientSocket);
                        Logger.LogInfo("Connection to the server has been established!");
                        IsConnected = true;
                        OnServerConnected?.Invoke();
                        await Receive(ClientSocket, async (result, buffer) =>
                        {
                            if (result.MessageType == WebSocketMessageType.Text)
                            {
                                string message = Encoding.UTF8.GetString(buffer).Replace("\0", string.Empty);
                                try { SocketHandler.Receive(ClientSocket, result, JObject.Parse(message)); }
                                catch { }
                            }
                            else if (result.MessageType == WebSocketMessageType.Close) await SocketHandler.OnDisconnected(ClientSocket);
                        });
                    }
                }
            }
        }

        private async Task Receive(ClientWebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            ArraySegment<byte> buffer = new(new byte[References.MaxSocketBufferSize]);
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    handleMessage(result, buffer.ToArray());
                }
            }
            catch { await SocketHandler.OnDisconnected(socket); }
        }
    }
}