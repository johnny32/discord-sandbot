using System.Threading.Tasks;
using DiscordSandbot.Discord;
using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace DiscordSandbot
{
    public class App
    {
        private readonly Configuration _configuration;
        private readonly IDiscordMessageHandler _discordMessageHandler;

        public App(Configuration configuration, IDiscordMessageHandler discordMessageHandler)
        {
            _configuration = configuration;
            _discordMessageHandler = discordMessageHandler;
        }

        public async Task RunAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = _configuration.DiscordToken,
                TokenType = TokenType.Bot
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new[] { "!" },
                Services = Program.Services
            });

            commands.RegisterCommands<CommandHandler>();

            discord.MessageCreated += _discordMessageHandler.HandleMessageAsync;
            discord.MessageReactionAdded += _discordMessageHandler.HandleAddReactionAsync;

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}