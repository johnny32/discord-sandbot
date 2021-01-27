using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace discord_sandbot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "MY TOKEN",
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
