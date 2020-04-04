using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GloomyTale.DiscordBot.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.MessageReceived += MessageReceivedAsync;
            _discord.ReactionAdded += OnReactAdded;
            _discord.ReactionRemoved += OnReactRemoved;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) { return; }
            if (message.Source != MessageSource.User) { return; }
            var argPos = 0;
            if (!message.HasStringPrefix("/", ref argPos) && !message.HasMentionPrefix(_discord.CurrentUser, ref argPos)) { return; }

            var context = new SocketCommandContext(_discord, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified || result.IsSuccess)
            {
                return;
            }

            await context.Channel.SendMessageAsync($"error: {result}");
        }

        public async Task OnReactAdded(Cacheable<IUserMessage, ulong> _msg, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (channel.Name == "language-chooise")
            {
                var user = _discord.Guilds.First().GetUser(reaction.UserId);
                var roles = _discord.Guilds.First().Roles;
                IRole role = null;
                switch (reaction.Emote.Name)
                {
                    case "🇮🇹":
                            role = roles.Where(r => r.Name == "ITALIAN").First();
                        break;
                    case "🇹🇷":
                            role = roles.Where(r => r.Name == "TURKISH").First();
                        break;
                    case "🇪🇸":
                            role = roles.Where(r => r.Name == "SPANISH").First();
                        break;
                    case "🇩🇪":
                            role = roles.Where(r => r.Name == "GERMAN").First();
                        break;
                    case "🇫🇷":
                            role = roles.Where(r => r.Name == "FRENCH").First();
                        break;
                    case "🇵🇱":
                            role = roles.Where(r => r.Name == "POLISH").First();
                        break;
                }
                
                if (role == null)
                    return;
                await user.AddRoleAsync(role);
            }
        }

        public async Task OnReactRemoved(Cacheable<IUserMessage, ulong> _msg, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (channel.Name == "language-chooise")
            {
                var user = _discord.Guilds.First().GetUser(reaction.UserId);
                var roles = _discord.Guilds.First().Roles;
                IRole role = null;
                switch(reaction.Emote.Name)
                {
                    case "🇮🇹":
                        if (user.Roles.Any(r => r.Name == "ITALIAN"))
                            role = roles.Where(r => r.Name == "ITALIAN").First();
                        break;
                    case "🇫🇷":
                        if (user.Roles.Any(r => r.Name == "TURKISH"))
                            role = roles.Where(r => r.Name == "TURKISH").First();
                        break;
                    case "🇪🇸":
                        if (user.Roles.Any(r => r.Name == "SPANISH"))
                            role = roles.Where(r => r.Name == "SPANISH").First();
                        break;
                    case "🇩🇪":
                        if (user.Roles.Any(r => r.Name == "GERMAN"))
                            role = roles.Where(r => r.Name == "GERMAN").First();
                        break;
                    case "🇹🇷":
                        if (user.Roles.Any(r => r.Name == "FRENCH"))
                            role = roles.Where(r => r.Name == "FRENCH").First();
                        break;
                    case "🇵🇱":
                        if (user.Roles.Any(r => r.Name == "POLISH"))
                            role = roles.Where(r => r.Name == "POLISH").First();
                        break;
                }

                if (role == null)
                    return;
                await user.RemoveRoleAsync(role);
            }
        }
    }
}
