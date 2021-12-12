using System.Net.WebSockets;

using UEESA.Client.Data.States;
using UEESA.Json.Client;
using UEESA.Json.External.RSI.Roadmap;
using UEESA.Sockets;

using Newtonsoft.Json.Linq;

namespace UEESA.Client.Sockets.Handlers
{
    public class StateSocketHandler : SocketHandler<ClientWebSocket>
    {
        public StateSocketHandler(ConnectionManager<ClientWebSocket> webSocketConnectionManager) : base(webSocketConnectionManager) { }

        public override void OnConnected(ClientWebSocket socket) => base.OnConnected(socket);

        public override void Receive(ClientWebSocket socket, WebSocketReceiveResult result, JObject message)
        {
            if (message.ContainsKey("datetime_sent") && message.ContainsKey("attributes") && message.ContainsKey("data_type") && message.ContainsKey("data") && message.Count == 4)
            {
                if (message["datetime_sent"].Type != JTokenType.Null && message["attributes"].Type != JTokenType.Null && message["data_type"].Type != JTokenType.Null)
                {
                    if (message["data_type"].ToString() == typeof(JRSI_Roadmap).Name)
                    {
                        JClient_SocketDataCapsule<JRSI_Roadmap> data = message.ToObject<JClient_SocketDataCapsule<JRSI_Roadmap>>();

                        if (data.Attributes.Contains(JClient_SocketDataCapsuleAttributes.GetRoadmapData.ToString())) Services.Get<ClientState>().NotifyRoadmapCardDataChange(data.Data, false);

                        if (data.Data != null)
                        {

                        }
                    }
                    else
                    {
                        JClient_SocketDataCapsule<object> data = message.ToObject<JClient_SocketDataCapsule<object>>();

                        // Lone Attributes
                    }
                }
            }
        }
    }
}
