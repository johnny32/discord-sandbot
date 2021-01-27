using System.Threading.Tasks;
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
            await Task.Delay(-1);

            /*var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "MY TOKEN",
                TokenType = TokenType.Bot
            });

            discord.MessageCreated += OnDiscordMessageCreated;

            await discord.ConnectAsync();
            await Task.Delay(-1);*/

            /*static async Task OnDiscordMessageCreated(MessageCreateEventArgs args)
            {
            if (args.Message.Content == "ping")
                await args.Message.RespondAsync("pong");
            }*/
        }
    }
}