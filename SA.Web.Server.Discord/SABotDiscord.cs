using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using SA.Web.Server.Discord.Commands;

namespace SA.Web.Server.Discord
{
    public static class SABotDiscord
    {
        internal static DiscordSocketClient DiscordClient { get; private set; }
        internal static string DebugChannelID { get; private set; }

        public static async void StartBot(string version, string token, string guildID, string debugChannelID)
        {
            DebugChannelID = debugChannelID;

            using (ServiceProvider services = ConfigureServices())
            {
                DiscordClient = new DiscordSocketClient();
                DiscordClient.Log += LogAsync;
                DiscordClient.JoinedGuild += async (SocketGuild guild) =>
                {
                    if (guild.Id != ulong.Parse(guildID)) await guild.LeaveAsync();
                };
                DiscordClient.UserJoined += async (SocketGuildUser user) =>
                {
                    if (!user.IsBot) await user.AddRoleAsync(user.Guild.Roles.First(x => x.Name == "Freelancers"));
                };
                DiscordClient.Log += LogAsync;

                await DiscordClient.LoginAsync(TokenType.Bot, token);
                await DiscordClient.StartAsync();
                await DiscordClient.SetGameAsync(version);

                await services.GetRequiredService<CommandModuleService>().InitializeAsync();

                static async Task LogAsync(LogMessage log)
                {
                    // Replace this with BSL.NET logger
                    Console.WriteLine(log.ToString());
                }
            }

            static ServiceProvider ConfigureServices()
            {
                return new ServiceCollection()
                    .AddSingleton<DiscordSocketClient>()
                    .AddSingleton<CommandService>()
                    .AddSingleton<CommandModuleService>()
                    .AddSingleton<HttpClient>()
                    .BuildServiceProvider();
            }
        }
    }
}
