using Discord;
using Discord.Commands;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using System.Threading.Tasks;

namespace GloomyTale.DiscordBot.Modules
{
    public class SendItemModule : ModuleBase<SocketCommandContext>
    {
        [Command("send-item")]
        [Name("send-item <characterName> <vnum> <amount>")]
        [Summary("Send an item in game")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SendItem(string characterName, short vnum, short amount)
        {
            if (!string.IsNullOrEmpty(characterName) && vnum > 0 && amount > 0)
            {
                DiscordServiceClient.Instance.SendItem(characterName, new DiscordItem()
                {
                    ItemVNum = vnum,
                    Amount = amount,
                });
            }
        }
    }
}
