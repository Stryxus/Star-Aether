using System;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SA.Bot.Discord.Commands
{
    public class CommandModuleService
    {
        private readonly CommandService commands;
        private readonly IServiceProvider services;

        public CommandModuleService(IServiceProvider serv)
        {
            commands = serv.GetRequiredService<CommandService>();
            services = serv;

            commands.CommandExecuted += CommandExecutedAsync;
            SABotDiscord.DiscordClient.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        public async Task MessageReceivedAsync(SocketMessage msg)
        {
            if (msg.Channel.Id != ulong.Parse(SABotDiscord.DebugChannelID)) return;
            if (!(msg is SocketUserMessage message)) return;
            if (!VerifyCommand(ref message, out int pos)) return;
            await commands.ExecuteAsync(new SocketCommandContext(SABotDiscord.DiscordClient, message), pos, services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified) return;
            if (result.IsSuccess) return;
            await context.Channel.SendMessageAsync("An error occured: " + result);
        }

        public static bool VerifyCommand(ref SocketUserMessage msg, out int pos)
        {
            pos = 0;
            return !msg.Author.IsBot && msg.HasCharPrefix('!', ref pos);
        }
    }
}
