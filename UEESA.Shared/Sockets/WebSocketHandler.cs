using System.Net.WebSockets;
using System.Text;

using UEESA.Data.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UEESA.Sockets
{
    public abstract class WebSocketHandler<T> where T : WebSocket
    {
        protected ConnectionManager<T> WebSocketConnectionManager { get; set; }

        public WebSocketHandler(ConnectionManager<T> webSocketConnectionManager) => WebSocketConnectionManager = webSocketConnectionManager;

        public virtual void OnConnected(T socket) => WebSocketConnectionManager.AddSocket(socket);

        public virtual async Task OnDisconnected(T socket) => await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));

        public async void SendMessageAsync<M>(T socket, UEESA_Json_StateSocketDataCapsule<M> message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            if (socket == null || socket.State != WebSocketState.Open) return;
            else await socket.SendAsync(msg, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public void SendMessageToAllAsync<M>(UEESA_Json_StateSocketDataCapsule<M> message)
        {
            foreach (KeyValuePair<Guid, T> pair in WebSocketConnectionManager.GetAll()) if (pair.Value.State == WebSocketState.Open) SendMessageAsync(pair.Value, message);
        }

        public abstract void Receive(T socket, WebSocketReceiveResult result, JObject message);
    }
}
