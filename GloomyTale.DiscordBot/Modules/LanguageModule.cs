using Discord.Commands;
using Discord.WebSocket;
using GloomyTale.DiscordBot.Enumerations;
using System.Linq;
using System.Threading.Tasks;

namespace GloomyTale.DiscordBot.Modules
{
    public class TranslationModule : ModuleBase<SocketCommandContext>
    {
        [Command("lang")]
        [Name("lang <language>")]
        [Summary("Give the language roles (You have to be on #language-chooise)")]
        public async Task Translation(RegionType region)
        {
            if (Context.Channel.Name == "language-chooise")
            {
                if (region != RegionType.FRENCH || region != RegionType.GERMAN || region != RegionType.ITALIAN || region != RegionType.POLISH
                    || region != RegionType.RUSSIAN || region != RegionType.SPANISH || region != RegionType.TURKISH)
                {
                    await ReplyAsync("the role doesn't exist.");
                    return;
                }
                var role = Context.Guild.Roles.FirstOrDefault(r => r.Name == $"{region.ToString()}");
                if (role == null)
                {
                    await ReplyAsync("the role doesn't exist.");
                    return;
                }

                var user = (SocketGuildUser)Context.User;
                if (role.Members.Any(r => r.Username == user.Username))
                {
                    await user.RemoveRoleAsync(role);
                    await ReplyAsync($"{user.Username} lost the role {role.Name}.");
                }
                else
                {
                    await user.AddRoleAsync(role);
                    await ReplyAsync($"{user.Username} got the role {role.Name}.");
                }
            }
            else
            {
                await ReplyAsync("You have to be on #language-chooise.");
            }
        }
    }
}
