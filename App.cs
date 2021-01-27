using System.Threading.Tasks;
using DiscordSandbot.Discord;
using DSharpPlus;

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

            discord.MessageCreated += _discordMessageHandler.HandleMessageAsync;

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}