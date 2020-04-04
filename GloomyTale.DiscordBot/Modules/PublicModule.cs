using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OpenNos.Master.Library.Client;
using System.Threading.Tasks;

namespace GloomyTale.DiscordBot.Modules
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        [Command("home")]
        [Name("home <characterName>")]
        [Summary("STAFF: Teleport a character to GloomyVille")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Home(string characterName)
        {
            DiscordServiceClient.Instance.Home(characterName);
            await ReplyAsync("In restarting...");
        }

        /*[Command("clear")]
        [Name("clear")]
        [Summary("clear all messages")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task Clear()
        {
            var clone = (Context.Channel as ITextChannel)?.CloneChannelAsync();
            if (clone != null)
            {
                await clone;
            }
        }  */
    }
}
