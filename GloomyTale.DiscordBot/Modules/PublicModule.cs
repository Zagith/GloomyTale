﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GloomyTale.DiscordBot.Extensions;
using GloomyTale.DiscordBot.Services;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
