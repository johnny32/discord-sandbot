using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;

namespace DiscordSandbot
{
    public class App
    {
        private readonly Configuration _configuration;
        public App(Configuration configuration)
        {
            _configuration = configuration;
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

        static async Task OnDiscordMessageCreated(MessageCreateEventArgs args)
        {
            if (args.Message.Content == "ping")
                await args.Message.RespondAsync("pong");
        }
    }
}