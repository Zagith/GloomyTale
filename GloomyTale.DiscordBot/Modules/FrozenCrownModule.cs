using Discord;
using Discord.Commands;
using OpenNos.Master.Library.Client;
using System;
using System.Threading.Tasks;

namespace GloomyTale.DiscordBot.Modules
{
    public class FrozenCrownModule : ModuleBase<SocketCommandContext>
    {
        [Command("act4stat")]
        [Name("act4stat")]
        [Summary("Give the act4 percentage stat (You have to be on #bot-commands) WARN: It will update every minute")]
        public async Task Act4Statistic()
        {
            if (Context.Channel.Name == "bot-commands")
            {
                Tuple<int, int> stats = DiscordServiceClient.Instance.GetAct4Stat();
                if (stats != null)
                {
                    string message = "Act4 stats:";
                    EmbedBuilder angel = new EmbedBuilder
                    {
                        Color = Color.Gold,
                        ImageUrl = "https://cdn.discordapp.com/attachments/420774865369300993/650332113576525844/angel.png",
                        Title = "Faction Angel",
                        Description = $"Percentage: {stats.Item1}%"
                    };
                    await ReplyAsync(message, false, angel.Build());
                    EmbedBuilder demon = new EmbedBuilder
                    {
                        ImageUrl = "https://cdn.discordapp.com/attachments/420774865369300993/650332109025574935/demon.png",
                        Title = "Faction Demon",
                        Description = $"Percentage: {stats.Item2}%"
                    };

                    await ReplyAsync("", false, demon.Build());
                }
                else
                {
                    await ReplyAsync("Frozen Crown channel OFFLINE.");
                }
            }
            else
            {
                await ReplyAsync("You have to be on #bot-commands.");
            }
        }
    }
}
