using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text;

using UEESA.Client.Data.States;
using UEESA.Client.Sockets.Handlers;
using UEESA.Sockets;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UEESA.Client.Sockets
{
    internal class WebSocketManagerMiddleware
    {
        public WebSocketHandler<ClientWebSocket> SocketHandler { get; private set; }
        public ClientWebSocket ClientSocket { get; private set; }
        public bool IsConnected { get; private set; }
        public event Action OnServerConnected;
        public event Action OnServerConnectionError;

        internal WebSocketManagerMiddleware(StateSocketHandler handler)
        {
            SocketHandler = handler;
        }

        internal async Task Connect(ClientState state)
        {
            if (ClientSocket != null) return;
            else
            {
                ClientSocket = new ClientWebSocket();
                try
                {
                    await ClientSocket.ConnectAsync(new Uri("wss://staraether.com/state"), CancellationToken.None).ContinueWith(async (task) => await Continued());
                }
                catch (WebSocketException)
                {
                    await ClientSocket.ConnectAsync(new Uri("wss://localhost:5001/state"), CancellationToken.None).ContinueWith(async (task) => await Continued());
                }

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
                                string message = Encoding.UTF8.GetString(buffer);
                                message = message.Replace("\0", string.Empty);
                                try
                                {
                                    SocketHandler.Receive(ClientSocket, result, JObject.Parse(message));
                                }
                                catch (JsonReaderException) { }
                                return;
                            }
                            else if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await SocketHandler.OnDisconnected(ClientSocket);
                                return;
                            }
                        });
                    }
                    else OnServerConnectionError?.Invoke();
                }
            }
        }

        private async Task Receive(ClientWebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            ArraySegment<byte> buffer = new(new byte[Globals.MaxSocketBufferSize]);
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    handleMessage(result, buffer.ToArray());
                }
            }
            catch (WebSocketException)
            {
                await SocketHandler.OnDisconnected(socket);
            }
        }
    }
}
