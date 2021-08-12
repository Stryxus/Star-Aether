using System.Net.WebSockets;

using UEESA.Client.Data.States;
using UEESA.Data.Bson.Roadmap;
using UEESA.Data.Json;
using UEESA.Sockets;

using Newtonsoft.Json.Linq;

namespace UEESA.Client.Sockets.Handlers
{
    public class StateSocketHandler : WebSocketHandler<ClientWebSocket>
    {
        public StateSocketHandler(ConnectionManager<ClientWebSocket> webSocketConnectionManager) : base(webSocketConnectionManager) { }

        public override void OnConnected(ClientWebSocket socket) => base.OnConnected(socket);

        public override void Receive(ClientWebSocket socket, WebSocketReceiveResult result, JObject message)
        {
            if (message.ContainsKey("datetime_sent") && message.ContainsKey("attributes") && message.ContainsKey("data_type") && message.ContainsKey("data") && message.Count == 4)
            {
                if (message["datetime_sent"].Type != JTokenType.Null && message["attributes"].Type != JTokenType.Null && message["data_type"].Type != JTokenType.Null)
                {
                    if (message["data_type"].ToString() == typeof(UEESA_Bson_Roadmap).Name)
                    {
                        UEESA_Json_StateSocketDataCapsule<UEESA_Bson_Roadmap> data = message.ToObject<UEESA_Json_StateSocketDataCapsule<UEESA_Bson_Roadmap>>();

                        if (data.attributes.Contains(StateSocketDataCapsuleAttributes.GetRoadmapData.ToString())) Services.Get<ClientState>().NotifyRoadmapCardDataChange(data.data, false);

                        if (data.data != null)
                        {

                        }
                    }
                    else
                    {
                        UEESA_Json_StateSocketDataCapsule<object> data = message.ToObject<UEESA_Json_StateSocketDataCapsule<object>>();

                        // Lone Attributes
                    }
                }
            }
        }
    }
}
