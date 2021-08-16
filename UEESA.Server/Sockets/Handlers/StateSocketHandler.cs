using System.Net.WebSockets;

using UEESA.Server.Data;
using UEESA.Data.Bson.Roadmap;
using UEESA.Data.Json;
using UEESA.Sockets;

using Newtonsoft.Json.Linq;

namespace UEESA.Server.Sockets.Handlers
{
    public class StateSocketHandler : WebSocketHandler<WebSocket>
    {
        public StateSocketHandler(ConnectionManager<WebSocket> webSocketConnectionManager) : base(webSocketConnectionManager) { }

        public override void OnConnected(WebSocket socket) => base.OnConnected(socket);

        public override void Receive(WebSocket socket, WebSocketReceiveResult result, JObject message)
        {
            if (message.ContainsKey("datetime_sent") && message.ContainsKey("attributes") && message.ContainsKey("data_type") && message.ContainsKey("data") && message.Count == 4)
            {
                if (message["datetime_sent"].Type != JTokenType.Null && message["attributes"].Type != JTokenType.Null && message["data_type"].Type != JTokenType.Null)
                {
                    if (message["data_type"].ToString() == typeof(UEESA_Bson_Roadmap).Name)
                    {

                    }
                    else
                    {
                        UEESA_Json_StateSocketDataCapsule<object> data = message.ToObject<UEESA_Json_StateSocketDataCapsule<object>>();

                        if (data.attributes.Contains(StateSocketDataCapsuleAttributes.GetRoadmapData.ToString())) SendMessageAsync(socket, new UEESA_Json_StateSocketDataCapsule<UEESA_Bson_Roadmap>
                        {
                            attributes = new List<string>() { StateSocketDataCapsuleAttributes.GetRoadmapData.ToString() },
                            data = Services.Get<RSIRoadmapScraper>().Roadmap_Data
                        });
                    }
                }
            }
        }
    }
}
