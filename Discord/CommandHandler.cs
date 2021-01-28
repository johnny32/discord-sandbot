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
            else
            {
                await context.Message.RespondAsync($"Hold up! Only {_botAdminUsername} can use this command!");
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
            else
            {
                await context.Message.RespondAsync($"Hold up! Only {_botAdminUsername} can use this command!");
            }
        }

        [Command("listEmojis")]
        [Description("Lists all the custom emojis and ranks them by most used.")]
        public async Task ListEmojisAsync(CommandContext context, string arg = null)
        {
            try
            {
                if (string.IsNullOrEmpty(arg) || arg.Contains(' '))
                    await ListAllEmojisAsync(context);
                else if (arg.StartsWith('<') && arg.EndsWith('>') && arg.Count(c => c == ':') == 2)
                {
                    string[] parts = arg.Substring(1, arg.Length - 1).Split(':');
                    await ListAllUsersOfEmojiAsync(context, $":{parts[1]}:");
                }
                else
                    await ListAllEmojisOfUserAsync(context, arg);
            }
            catch (Exception e)
            {
                await context.RespondAsync(e.Message);
            }
        }

        private async Task ListAllEmojisAsync(CommandContext context)
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

        private async Task ListAllUsersOfEmojiAsync(CommandContext context, string emojiId)
        {
            var results = await _database.GetAllUsersOfEmojiAsync(emojiId);

            if (!results.Any())
                return;

            var sb = new StringBuilder();
            DiscordEmoji emoji = DiscordEmoji.FromName(context.Client, emojiId);
            sb.AppendLine($"Total uses of {emoji}: {results.Count()}");

            var groupByUser = results
                .GroupBy(row => row.Username)
                .OrderByDescending(g => g.Count());

            foreach (var group in groupByUser)
            {
                DateTime lastUsed = group
                    .Select(row => DateTime.Parse(row.MessageTimestamp))
                    .OrderByDescending(g => g)
                    .First();

                sb.AppendLine($"Used by {group.Key} a total of {group.Count()} times. It was used last on {lastUsed.ToString("dd/MM/yyyy HH:mm:ss")}.");
            }

            await context.RespondAsync(sb.ToString());
        }

        private async Task ListAllEmojisOfUserAsync(CommandContext context, string username)
        {
            var results = await _database.GetAllEmojisOfUserAsync(username);

            if (!results.Any())
                return;

            var sb = new StringBuilder();
            sb.AppendLine($"{username} has used the emojis:");

            var groupByEmoji = results
                .GroupBy(row => row.EmojiId)
                .OrderByDescending(g => g.Count());

            foreach (var group in groupByEmoji)
            {
                DiscordEmoji emoji = DiscordEmoji.FromName(context.Client, group.Key);

                DateTime lastUsed = group
                    .Select(row => DateTime.Parse(row.MessageTimestamp))
                    .OrderByDescending(g => g)
                    .First();

                sb.AppendLine($"{emoji} a total of {group.Count()} times. It was used last on {lastUsed.ToString("dd/MM/yyyy HH:mm:ss")}.");
            }

            await context.RespondAsync(sb.ToString());
        }

        [Command("deleteEmoji")]
        [Description("Deletes all usage of a specific emoji")]
        public async Task DeleteEmojiAsync(CommandContext context, string arg)
        {
            if (context.Message.Author.Username == _botAdminUsername)
            {
                if (arg.StartsWith('<') && arg.EndsWith('>') && arg.Count(c => c == ':') == 2)
                {
                    string[] parts = arg.Substring(1, arg.Length - 1).Split(':');
                    await _database.DeleteEmojiAsync($":{parts[1]}:");
                }
                else
                {
                    //TODO Global exception handler?
                    //throw new FormatException("Bad emoji format");
                    await context.Message.RespondAsync("Bad emoji format");
                }
            }
            else
            {
                await context.Message.RespondAsync($"Hold up! Only {_botAdminUsername} can use this command!");
            }
        }
    }
}