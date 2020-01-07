﻿using Discord;
using Discord.Audio;
using Discord.Commands;
using GloomyTale.DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;

namespace GloomyTale.DiscordBot.Modules
{
    public class MusicModule : ModuleBase<ShardedCommandContext>
    {
        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }

        public Dictionary<ulong, CancellationTokenSource> pauseCancelTokens = new Dictionary<ulong, CancellationTokenSource>();
        public Dictionary<ulong, bool> pauseBools = new Dictionary<ulong, bool>();
        private async Task PausableCopyToAsync(Stream source, Stream destination, ulong guildId, int buffersize)
        {
            byte[] buffer = new byte[buffersize];
            int count;
            pauseCancelTokens.TryGetValue(guildId, out CancellationTokenSource token);

            while ((count = await source.ReadAsync(buffer, 0, buffersize, token.Token).ConfigureAwait(false)) > 0)
            {
                pauseBools.TryGetValue(guildId, out bool _pause);
                if (_pause)
                {
                    try
                    {
                        await Task.Delay(Timeout.Infinite, token.Token);
                    }
                    catch (OperationCanceledException) { }
                }

                await destination.WriteAsync(buffer, 0, count, token.Token).ConfigureAwait(false);
            }
        }

        public Dictionary<ulong, AudioOutStream> aStreams = new Dictionary<ulong, AudioOutStream>();

        [Command("play", RunMode = RunMode.Async), Alias("music"), RequireContext(ContextType.Guild)]
        public async Task<RuntimeResult> MusicAsync([Remainder] string search)
        {
            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (Context.Guild.CurrentUser.VoiceChannel != null) { return ResultService.FromError("I'm already in a voice channel!"); }
            if (channel == null) { return ResultService.FromError("You need to be in a voice channel to do that."); }

            var message = await ReplyAsync($"searching for `{search}` on YouTube...");

            var client = new YoutubeClient();
            var vidSearch = await client.SearchVideosAsync(search, 1) as List<YoutubeExplode.Models.Video>;
            var searchArray = vidSearch.ToArray();
            if (searchArray.Length <= 0)
                return ResultService.FromError($"No videos could be found for the query `{search}`");
            if (searchArray[0].Duration > TimeSpan.FromHours(1))
                return ResultService.FromError("Videos must be no longer than one hour be played.");

            await message.ModifyAsync(msg => msg.Content = $"Found video! (`{searchArray[0].Title}` uploaded by `{searchArray[0].Author}`)\nPreparing audio...");
            var info = await client.GetVideoMediaStreamInfosAsync(searchArray[0].Id);
            var converter = new YoutubeConverter(client);
            await converter.DownloadVideoAsync(info, $"song_{Context.Guild.Id}.opus", "opus");

            await message.ModifyAsync(msg => msg.Content = $"Joining channel and playing `{searchArray[0].Title}`");
            var aClient = await channel.ConnectAsync();

            using (var ffmpeg = CreateStream($"song_{Context.Guild.Id}.opus"))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = aClient.CreatePCMStream(AudioApplication.Mixed))
            {
                try
                {
                    aStreams.Add(Context.Guild.Id, discord);
                    pauseCancelTokens.Add(Context.Guild.Id, new CancellationTokenSource());
                    pauseBools.Add(Context.Guild.Id, false);
                    await PausableCopyToAsync(output, discord, Context.Guild.Id, 4096);
                }

                finally { await discord.FlushAsync(); }
            }

            await Context.Guild.CurrentUser.VoiceChannel.DisconnectAsync();
            return ResultService.FromSuccess();
        }

        [Command("disconnect", RunMode = RunMode.Async), Alias("leave", "dc")]
        public async Task<RuntimeResult> DiscAsync()
        {
            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (Context.Guild.CurrentUser.VoiceChannel == null) { return ResultService.FromError("I'm not in a voice channel!"); }
            if (channel != Context.Guild.CurrentUser.VoiceChannel) { return ResultService.FromError($"You need to be in my voice channel (`{Context.Guild.CurrentUser.VoiceChannel.Name}`) to do that."); }
            if (aStreams.TryGetValue(Context.Guild.Id, out AudioOutStream aStream)) { }
            else
                return ResultService.FromStrangeError("Audio client is missing from dictionary aClients.");
            await aStream.FlushAsync();
            await Context.Guild.CurrentUser.VoiceChannel.DisconnectAsync();
            return ResultService.FromSuccess();
        }
    }
}
