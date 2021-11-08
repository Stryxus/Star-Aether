using System.Collections.Generic;
using System.Net.WebSockets;

using UEESA.Server.Data;
using UEESA.Json.Client;
using UEESA.Json.External.RSI.Roadmap;
using UEESA.Sockets;

using Newtonsoft.Json.Linq;

namespace UEESA.Server.Sockets.Handlers
{
    public class StateSocketHandler : SocketHandler<WebSocket>
    {
        public StateSocketHandler(ConnectionManager<WebSocket> webSocketConnectionManager) : base(webSocketConnectionManager) { }

        public override void OnConnected(WebSocket socket) => base.OnConnected(socket);

        public override void Receive(WebSocket socket, WebSocketReceiveResult result, JObject message)
        {
            if (message.ContainsKey("datetime_sent") && message.ContainsKey("attributes") && message.ContainsKey("data_type") && message.ContainsKey("data") && message.Count == 4)
            {
                if (message["datetime_sent"].Type != JTokenType.Null && message["attributes"].Type != JTokenType.Null && message["data_type"].Type != JTokenType.Null)
                {
                    if (message["data_type"].ToString() == typeof(JRSI_Roadmap).Name)
                    {

                    }
                    else
                    {
                        JClient_SocketDataCapsule<object> data = message.ToObject<JClient_SocketDataCapsule<object>>();

                        if (data.Attributes.Contains(JClient_SocketDataCapsuleAttributes.GetRoadmapData.ToString())) SendMessageAsync(socket, new JClient_SocketDataCapsule<JRSI_Roadmap>
                        {
                            Attributes = new List<string>() { JClient_SocketDataCapsuleAttributes.GetRoadmapData.ToString() },
                            Data = Services.Get<RSIRoadmapScraper>().Roadmap_Data
                        });
                    }
                }
            }
        }
    }
}
