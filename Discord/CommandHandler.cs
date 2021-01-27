using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DiscordSandbot.Database;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DiscordSandbot.Discord
{
    public class CommandHandler : BaseCommandModule
    {
        private readonly IDatabaseService _database;
        private readonly string _botAdminUsername = "jooohnny32"; //For private functions
        private readonly string _botUsername = "Sandbot";

        public CommandHandler(IDatabaseService database)
        {
            _database = database;
        }

        [Command("setup")]
        public async Task SetupCommandAsync(CommandContext context)
        {
            await context.RespondAsync("Greetings");
        }

        [Command("help-commands")]
        public async Task ShowListOfCommandsAsync(CommandContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("List of commands: ");
            sb.AppendLine();
            sb.AppendLine("  -  !setup: Initializes the database if it isn't already initialized.");
            sb.AppendLine("  -  !destroy: Destroys the tables and wipes the data.");

            await context.RespondAsync(sb.ToString());
        }

        [Command("listEmojis")]
        public async Task ListEmojisAsync(CommandContext context)
        {
            try
            {
                var results = await _database.GetAllEmojisAsync();

                if (!results.Any())
                    return;

                var sb = new StringBuilder();
                foreach (var row in results)
                {
                    var emoji = DiscordEmoji.FromName(context.Client, row.EmojiId);
                    sb.AppendLine($"{row.Username} - {emoji} - {row.MessageTimestamp}");
                }

                await context.RespondAsync(sb.ToString());
            }
            catch (Exception e)
            {
                await context.RespondAsync(e.Message);
            }
        }
    }
}