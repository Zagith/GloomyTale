﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GloomyTale.DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OpenNos.Core;
using OpenNos.Master.Library.Client;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace GloomyTale.DiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            Console.Title = $"DevSlix Discord Bot Server";
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string text = $"DISCORD BOT SERVER v{fileVersionInfo.ProductVersion}dev - by DevSlix Team";
            int offset = (Console.WindowWidth / 2) + (text.Length / 2);
            string separator = new string('=', Console.WindowWidth);
            Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);

            Initialize();
        }
        static void Initialize()
            => new Program()
                .MainAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

        public async Task MainAsync()
        {
            if (DiscordServiceClient.Instance.Authenticate(ConfigurationManager.AppSettings["MasterAuth"]))
            {
                Logger.Info(Language.Instance.GetMessageFromKey("API_INITIALIZED"));
            }
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // Tokens should be considered secret data, and never hard-coded.
                await client.LoginAsync(TokenType.Bot, "NjQ3MzQ4OTczMjc0NjYwODY1.Xm58lQ.Xr_MFgfugcybB0lf4IlnzpAxKbA");
                await client.StartAsync();

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}
