using System.Threading.Tasks;
using DiscordSandbot.Database;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;

namespace DiscordSandbot
{
    public class App
    {
        private readonly Configuration _configuration;
        private readonly IDatabaseService _database;

        public App(Configuration configuration, IDatabaseService database)
        {
            _configuration = configuration;
            _database = database;
        }

        public async Task RunAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = _configuration.DiscordToken,
                TokenType = TokenType.Bot
            });

            discord.MessageCreated += OnDiscordMessageCreated;

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private async Task OnDiscordMessageCreated(MessageCreateEventArgs args)
        {
            if (args.Message.Content == "ping")
                await args.Message.RespondAsync("pong");

            if (args.Message.Content == "!setup")
                await _database.SetupAsync();

            if (args.Message.Content == "!destroy" && args.Message.Author.Username == "jooohnny32")
                await _database.DestroyAsync();
        }
    }
}