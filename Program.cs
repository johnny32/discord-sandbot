using System;
using System.Threading.Tasks;
using DSharpPlus;

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

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
