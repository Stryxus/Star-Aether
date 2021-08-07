using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace UEESA.Shared.Sockets
{
    public abstract class WebSocketHandler<T> where T : WebSocket
    {
        protected ConnectionManager<T> WebSocketConnectionManager { get; set; }

        public WebSocketHandler(ConnectionManager<T> webSocketConnectionManager) => WebSocketConnectionManager = webSocketConnectionManager;

        public virtual void OnConnected(T socket) => WebSocketConnectionManager.AddSocket(socket);

        public virtual async Task OnDisconnected(T socket) => await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));

        public async void SendMessageAsync(T socket, string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes("MBEGIN|" + message + "|MEND");
            if (socket == null || socket.State != WebSocketState.Open) return;
            else await socket.SendAsync(msg, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public void SendMessageToAllAsync(string message)
        {
            foreach (var pair in WebSocketConnectionManager.GetAll()) if (pair.Value.State == WebSocketState.Open) SendMessageAsync(pair.Value, message);
        }

        public abstract void Receive(T socket, WebSocketReceiveResult result, string message);
    }
}
