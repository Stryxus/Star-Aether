using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using UEESA.Shared.Sockets;

namespace UEESA.Server.Sockets
{
    public class WebSocketManagerMiddleware
    {
        private WebSocketHandler<WebSocket> SocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next, WebSocketHandler<WebSocket> webSocketHandler) => SocketHandler = webSocketHandler;

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest) return;
            WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
            SocketHandler.OnConnected(socket);
            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer);
                    message = message.Replace("\0", string.Empty);
                    try
                    {
                        SocketHandler.Receive(socket, result, JObject.Parse(message));
                    }
                    catch (JsonReaderException) { }
                    return;
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await SocketHandler.OnDisconnected(socket);
                    return;
                }
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[Globals.MaxSocketBufferSize]);
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

    public static class WebSocketManagerMiddlewareExtensions
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, PathString path, WebSocketHandler<WebSocket> handler) => app.Map(path, (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>(handler));

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<ConnectionManager<WebSocket>>();
            foreach (Type type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler<WebSocket>)) services.AddScoped(type);
            }
            return services;
        }
    }
}
