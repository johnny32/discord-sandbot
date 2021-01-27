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
        [Description("Initializes the database if it isn't already initialized.")]
        public async Task SetupCommandAsync(CommandContext context)
        {
            if (context.Message.Author.Username == _botAdminUsername)
            {
                await _database.SetupAsync();
            }
        }

        [Command("destroy")]
        [Description("Destroys the tables and wipes the data.")]
        public async Task DestroyCommandAsync(CommandContext context)
        {
            if (context.Message.Author.Username == _botAdminUsername)
            {
                await _database.DestroyAsync();
            }
        }

        [Command("listEmojis")]
        [Description("Lists all the custom emojis and ranks them by most used.")]
        public async Task ListEmojisAsync(CommandContext context)
        {
            try
            {
                var results = await _database.GetAllEmojisAsync();

                if (!results.Any())
                    return;

                var sb = new StringBuilder();

                var groupByEmoji = results
                    .GroupBy(row => row.EmojiId)
                    .OrderByDescending(g => g.Count());

                foreach (var group in groupByEmoji)
                {
                    DiscordEmoji emoji = DiscordEmoji.FromName(context.Client, group.Key);
                    int totalTimes = group.Count();
                    string mostFrequentUser = group
                        .GroupBy(row => row.Username)
                        .OrderByDescending(g => g.Count())
                        .First()
                        .Key;
                    DateTime lastUsed = group
                        .Select(row => DateTime.Parse(row.MessageTimestamp))
                        .OrderByDescending(g => g)
                        .First();

                    sb.AppendLine($"{emoji} was used {totalTimes} times. The user that uses it the most is {mostFrequentUser}, and it was used last on {lastUsed.ToString("dd/MM/yyyy HH:mm:ss")}.");
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