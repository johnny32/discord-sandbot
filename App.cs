using System.Threading.Tasks;
using DiscordSandbot.Database;
using DiscordSandbot.Discord;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;

namespace DiscordSandbot
{
    public class App
    {
        private readonly ILogger _logger;
        private readonly Configuration _configuration;
        private readonly IDiscordMessageHandler _discordMessageHandler;
        private readonly IDatabaseService _database;

        public App(ILogger<App> logger,
            Configuration configuration,
            IDiscordMessageHandler discordMessageHandler,
            IDatabaseService database)
        {
            _logger = logger;
            _configuration = configuration;
            _discordMessageHandler = discordMessageHandler;
            _database = database;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Sandbot startup");

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = _configuration.DiscordToken,
                TokenType = TokenType.Bot
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new[] { _configuration.CommandPrefix },
                Services = Program.Services
            });

            commands.RegisterCommands<CommandHandler>();

            discord.MessageCreated += _discordMessageHandler.HandleMessageAsync;
            discord.MessageReactionAdded += _discordMessageHandler.HandleAddReactionAsync;
            discord.MessageReactionRemoved += _discordMessageHandler.HandleRemoveReactionAsync;

            await _database.SetupAsync();

            await discord.ConnectAsync();

            await Task.Delay(-1);

            _logger.LogCritical("Sandbot shutdown!");
        }
    }
}