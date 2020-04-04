using Discord;
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
        [Name("lang")]
        [Summary("STAFF: Give sent the message for add the languages")]
        public async Task Translation()
        {
            if (Context.Channel.Name == "language-chooise")
            {
                EmbedBuilder demon = new EmbedBuilder
                {
                    Title = "Language Chooise",
                    Description = "React an emoji to receive the role"
                };
                
                var message = await ReplyAsync("", false, demon.Build());
                var emote = new Emoji("🇮🇹");
                var emote1 = new Emoji("🇪🇸");
                var emote2 = new Emoji("🇫🇷");
                var emote3 = new Emoji("🇩🇪");
                var emote4 = new Emoji("🇵🇱");
                var emote5 = new Emoji("🇹🇷");
                await message.AddReactionAsync(emote);
                await message.AddReactionAsync(emote1);
                await message.AddReactionAsync(emote2);
                await message.AddReactionAsync(emote3);
                await message.AddReactionAsync(emote4);
                await message.AddReactionAsync(emote5);
            }
            else
            {
                await ReplyAsync("You have to be on #language-chooise.");
            }
        }
    }
}
