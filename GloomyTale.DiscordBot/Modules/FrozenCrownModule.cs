using Discord;
using Discord.Commands;
using GloomyTale.DiscordBot.Extensions;
using System.Threading.Tasks;

namespace GloomyTale.DiscordBot.Modules
{
    public class FrozenCrownModule : ModuleBase<SocketCommandContext>
    {
        [Command("act4stat")]
        [Name("act4stat")]
        [Summary("Give the act4 percentage stat (You have to be on #bot-announcement)")]
        public async Task Act4Statistic()
        {
            if (Context.Channel.Name == "bot-announcement")
            {
                string message = "Act4 stats:";
                EmbedBuilder angel = new EmbedBuilder
                {
                    Color = Color.Gold,
                    ImageUrl = "https://cdn.discordapp.com/attachments/420774865369300993/650332113576525844/angel.png",
                    Title = "Faction Angel",
                    Description = $"Percentage: {FrozenCrownExtension.Instance.SendAct4Stat(1)}%"
                };
                await ReplyAsync(message, false, angel.Build());
                EmbedBuilder demon = new EmbedBuilder
                {
                    ImageUrl = "https://cdn.discordapp.com/attachments/420774865369300993/650332109025574935/demon.png",
                    Title = "Faction Demon",
                    Description = $"Percentage: {FrozenCrownExtension.Instance.SendAct4Stat(2)}%"
                };

                await ReplyAsync("", false, demon.Build());
            }
        }
    }
}
