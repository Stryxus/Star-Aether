using System;
using System.Net.WebSockets;

using SA.Web.Client.Data.States;
using SA.Web.Shared.WebSockets;
using SA.Web.Shared.Json;

using Newtonsoft.Json;

namespace SA.Web.Client.WebSockets
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

                TryConvertJSON<NewsData>((data) => Services.Get<ClientState>().NotifyNewsDataChange(data, false));
                TryConvertJSON<RoadmapData>((data) => Services.Get<ClientState>().NotifyRoadmapCardDataChange(data, false));

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
            else if (message.StartsWith("TWITCH_"))
            {
                if (message.StartsWith("TWITCH_LOGO."))
                {
                    Services.Get<ClientState>().AddTwitchLogo(message[(message.IndexOf("TWITCH_NAME.") + 12)..],
                                                                message.Substring(0, message.IndexOf("TWITCH_NAME.")).Replace("TWITCH_LOGO.", string.Empty));
                }
            }
            return;
        }
    }
}
