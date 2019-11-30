using Discord;
using Discord.Commands;
using OpenNos.GameObject.Networking;
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
                if (ServerManager.Instance.ChannelId != 51 && ServerManager.Instance.Act4AngelStat != null && ServerManager.Instance.Act4DemonStat != null)
                {
                    string message = "Act4 stats:";
                    EmbedBuilder angel = new EmbedBuilder
                    {
                        Color = Color.Gold,
                        ImageUrl = "https://cdn.discordapp.com/attachments/420774865369300993/650332113576525844/angel.png",
                        Title = "Faction Angel",
                        Description = $"Percentage: {ServerManager.Instance.Act4AngelStat.Percentage / 100}%"
                    };
                    await ReplyAsync(message, false, angel.Build());
                    EmbedBuilder demon = new EmbedBuilder
                    {
                        ImageUrl = "https://cdn.discordapp.com/attachments/420774865369300993/650332109025574935/demon.png",
                        Title = "Faction Demon",
                        Description = $"Percentage: {ServerManager.Instance.Act4DemonStat.Percentage / 100}%"
                    };

                    await ReplyAsync(message, false, demon.Build());
                }
            }
        }
    }
}
