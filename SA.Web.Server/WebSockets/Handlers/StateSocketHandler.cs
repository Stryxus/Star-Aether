using System;
using System.Net.WebSockets;

using SA.Web.Shared.WebSockets;
using SA.Web.Shared.Json;
using SA.Web.Server.Data;

namespace SA.Web.Server.WebSockets
{
    public class StateSocketHandler : WebSocketHandler<WebSocket>
    {
        public StateSocketHandler(ConnectionManager<WebSocket> webSocketConnectionManager) : base(webSocketConnectionManager) { }

        public override void OnConnected(WebSocket socket)  => base.OnConnected(socket);

        public override async void Receive(WebSocket socket, WebSocketReceiveResult result, string message)
        {
            message = message.Replace("\0", string.Empty);

            if (message.StartsWith("CMD.") && Enum.TryParse(typeof(Commands), message.Replace("CMD.", string.Empty), out object cmd))
            {
                if ((Commands)cmd == Commands.GetRoadmapData) SendMessageAsync(socket, "JSON." + typeof(RoadmapData).Name + await Services.Get<MongoDBInterface>().GetRoadmapData());
                else if ((Commands)cmd == Commands.GetNewsData) SendMessageAsync(socket, "JSON." + typeof(NewsData).Name + await Services.Get<MongoDBInterface>().GetNewsData());
                else if ((Commands)cmd == Commands.GetChangelogData) SendMessageAsync(socket, "JSON." + typeof(ChangelogData).Name + await Services.Get<MongoDBInterface>().GetChangelogData());
                else if ((Commands)cmd == Commands.GetPhotographyData) SendMessageAsync(socket, "JSON." + typeof(MediaPhotographyData).Name + await Services.Get<MongoDBInterface>().GetPhotographyData());
            }
            else if (message.StartsWith("TWITCH_"))
            {
                message = message.Replace("TWITCH_", string.Empty);

                if (message.StartsWith("LOGO."))
                {
                    string name = message.Replace("LOGO.", string.Empty);
                    SendMessageAsync(socket, "TWITCH_LOGO." + await Services.Get<TwitchInterface>().GetLogoURL(name) + "TWITCH_NAME." + name);
                }
            }

            return;
        }
    }
}
