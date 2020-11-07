using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace SA.Bot.Discord.Commands
{
    public class CommandHelp : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        public async Task HelpAsync()
        {
            await Context.User.SendMessageAsync("Help command coming soon! Its more complicated than anyone thinks!").ConfigureAwait(false);
        }
    }
}
