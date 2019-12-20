using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using GloomyTale.DiscordBot.Services;
using OpenNos.Master.Library.Client;
using OpenNos.Core;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

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
            using (var services = ConfigureServices())
            {               
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // Tokens should be considered secret data, and never hard-coded.
                await client.LoginAsync(TokenType.Bot, "NjQ3MzQ4OTczMjc0NjYwODY1.XeJiGw.-puEdzyP-ExA0fyG9ScuplxxCy0");
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
