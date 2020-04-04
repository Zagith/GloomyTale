using Discord;
using Discord.Commands;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DiscordBot.Modules
{
    public class RebootModule : ModuleBase<SocketCommandContext>
    {
        [Command("reboot")]
        [Name("reboot")]
        [Summary("STAFF: Restart all channels")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Reboot()
        {
            DiscordServiceClient.Instance.RestartAll();
            await ReplyAsync("In restarting...");
        }
    }
}
