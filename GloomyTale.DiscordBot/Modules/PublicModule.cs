﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OpenNos.Master.Library.Client;
using System.Threading.Tasks;

namespace GloomyTale.DiscordBot.Modules
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        [Command("del-msg")]
        [Name("del-msg <amount>")]
        [Summary("STAFF: Deletes a specified amount of messages")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Delete(int amount)
        {
            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(messages);
        }

        [Command("reboot")]
        [Name("reboot")]
        [Summary("STAFF: Restart all channels")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Reboot()
        {
            DiscordServiceClient.Instance.RestartAll();
            await ReplyAsync("In restarting...");
        }

        [Command("home")]
        [Name("home")]
        [Summary("/home <characterName>/nSTAFF: Teleport a character to GloomyVille")]
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
