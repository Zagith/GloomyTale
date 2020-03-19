using Discord.Commands;
using Discord.WebSocket;
using GloomyTale.DiscordBot.Enumerations;
using System;
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
                if (region == RegionType.HELP)
                {
                    await ReplyAsync("roles: \nENGLISH,\nGERMAN,\nFRENCH,\nITALIAN,\nSPANISH,\nRUSSIAN,\nTURKISH,\nPOLISH");
                    return;
                }
                var role = Context.Guild.Roles.FirstOrDefault(r => r.Name == $"{region.ToString()}");
                if (role == null)
                {
                    await ReplyAsync("the role doesn't exist.");
                    await ReplyAsync("help: /lang help \nroles: \nENGLISH,\nGERMAN,\nFRENCH,\nITALIAN,\nSPANISH,\nRUSSIAN,\nTURKISH,\nPOLISH");
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
