using System;
using System.Net.WebSockets;

using UEESA.Client.Data.States;
using UEESA.Shared.WebSockets;
using UEESA.Json.Roadmap;

using Newtonsoft.Json;

namespace UEESA.Client.WebSockets
{
    public class StateSocketHandler : WebSocketHandler<ClientWebSocket>
    {
        public StateSocketHandler(ConnectionManager<ClientWebSocket> webSocketConnectionManager) : base (webSocketConnectionManager) { }

        public override void OnConnected(ClientWebSocket socket) => base.OnConnected(socket);

        public override void Receive(ClientWebSocket socket, WebSocketReceiveResult result, string message)
        {
            if (message.StartsWith("CMD.") && Enum.TryParse(typeof(Commands), message.Replace("CMD.", string.Empty), out object cmd))
            {

            }
            else if (message.StartsWith("JSON."))
            {
                message = message.Replace("JSON.", string.Empty);

                TryConvertJSON<RSI_Roadmap_State>((data) => Services.Get<ClientState>().NotifyRoadmapCardDataChange(data, false));

                void TryConvertJSON<T>(Action<T> conversion)
                {
                    Type type;
                    T data;
                    if (message.StartsWith((type = typeof(T)).Name))
                    {
                        message = message[type.Name.Length..];
                        try
                        {
                            if ((data = JsonConvert.DeserializeObject<T>(message)) != null) conversion.Invoke(data);
                        }
#if DEBUG || RELEASE_TEST
                        catch (JsonException e) 
                        {
                            Logger.LogDebug(e.Message).GetAwaiter().GetResult();
                        }
#else
                        catch (JsonException) {}
#endif
                    }
                }
            }
            return;
        }
    }
}
