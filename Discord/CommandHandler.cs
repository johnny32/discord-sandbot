using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DiscordSandbot.Database;
using DiscordSandbot.HarterQuotes;
using DiscordSandbot.HoffmanQuotes;
using DiscordSandbot.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace DiscordSandbot.Discord
{
    public class CommandHandler : BaseCommandModule
    {
        private readonly ILogger _logger;
        private readonly IDatabaseService _database;
        private readonly IHarterQuotesService _harterQuotesService;
        private readonly IHoffmanQuotesService _hoffmanQuotesService;
        private readonly Configuration _configuration;

        public CommandHandler(
            ILogger<CommandHandler> logger,
            IDatabaseService database,
            IHarterQuotesService harterQuotesService,
            IHoffmanQuotesService hoffmanQuotesService,
            Configuration configuration)
        {
            _logger = logger;
            _database = database;
            _harterQuotesService = harterQuotesService;
            _hoffmanQuotesService = hoffmanQuotesService;
            _configuration = configuration;
        }

        [Command("setup")]
        [Description("Initializes the database if it isn't already initialized.")]
        public async Task SetupCommandAsync(CommandContext context)
        {
            try
            {
                _logger.LogInformation($"{context.Message.Author.Username} used the command setup");
                if (context.Message.Author.Username == _configuration.BotAdminUsername)
                {
                    await _database.SetupAsync();
                }
                else
                {
                    await context.Message.RespondAsync($"Hold up! Only {_configuration.BotAdminUsername} can use this command!");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception on command setup");
            }
        }

        [Command("listEmojis")]
        [Description("Lists all the custom emojis and ranks them by most used.")]
        public async Task ListEmojisAsync(CommandContext context, string arg = null)
        {
            try
            {
                _logger.LogInformation($"{context.Message.Author.Username} used the command listEmojis with parameters \"{(string.IsNullOrEmpty(arg) ? "" : arg)}\"");
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
                _logger.LogError(e, $"Exception on command listEmojis (arg = \"{(arg == null ? "null" : arg)}\")");
            }
        }

        private async Task ListAllEmojisAsync(CommandContext context)
        {
            IEnumerable<LogEmoji> results = await _database.GetAllEmojisAsync();

            if (!results.Any())
                return;

            StringBuilder builder = new();

            IOrderedEnumerable<IGrouping<string, LogEmoji>> groupByEmoji = results
                .GroupBy(row => row.EmojiId)
                .OrderByDescending(g => g.Count());

            builder.AppendLine("Usage of all emojis:");

            foreach (IGrouping<string, LogEmoji> group in groupByEmoji)
            {
                DiscordEmoji emoji = DiscordEmoji.FromName(context.Client, group.Key);
                int totalTimes = group.Count();
                int totalTimesAsReaction = group.Count(emoji => emoji.IsReaction);
                string mostFrequentUser = group
                    .GroupBy(row => row.Username)
                    .OrderByDescending(g => g.Count())
                    .First()
                    .Key;
                DateTime lastUsed = group
                    .Select(row => row.MessageTimestamp)
                    .OrderByDescending(g => g)
                    .First();

                builder.AppendLine();
                builder.AppendLine($"{emoji}");
                builder.AppendLine($"Used {totalTimes} times ({totalTimesAsReaction} of them as a reaction)");
                builder.AppendLine($"Mainly used by {mostFrequentUser}");
                builder.AppendLine($"Used last on {lastUsed.ToString("G")}");

                if (builder.Length >= 1500)
                {
                    await context.RespondAsync(builder.ToString());
                    builder.Clear();
                }
            }

            if (builder.Length > 0)
                await context.RespondAsync(builder.ToString());
        }

        private async Task ListAllUsersOfEmojiAsync(CommandContext context, string emojiId)
        {
            IEnumerable<LogEmoji> results = await _database.GetAllUsersOfEmojiAsync(emojiId);

            if (!results.Any())
                return;

            StringBuilder builder = new();
            DiscordEmoji emoji = DiscordEmoji.FromName(context.Client, emojiId);
            builder.AppendLine($"Total uses of {emoji}: {results.Count()}");

            IOrderedEnumerable<IGrouping<string, LogEmoji>> groupByUser = results
                .GroupBy(row => row.Username)
                .OrderByDescending(g => g.Count());

            foreach (IGrouping<string, LogEmoji> group in groupByUser)
            {
                int totalTimes = group.Count();
                int totalTimesAsReaction = group.Count(emoji => emoji.IsReaction);
                DateTime lastUsed = group
                    .Select(row => row.MessageTimestamp)
                    .OrderByDescending(g => g)
                    .First();

                builder.AppendLine();
                builder.AppendLine($"Used by {group.Key} a total of {totalTimes} times ({totalTimesAsReaction} of them as a reaction)");
                builder.AppendLine($"Used last on {lastUsed.ToString("G")}");

                if (builder.Length >= 1500)
                {
                    await context.RespondAsync(builder.ToString());
                    builder.Clear();
                }
            }

            if (builder.Length > 0)
                await context.RespondAsync(builder.ToString());
        }

        private async Task ListAllEmojisOfUserAsync(CommandContext context, string username)
        {
            IEnumerable<LogEmoji> results = await _database.GetAllEmojisOfUserAsync(username);

            if (!results.Any())
                return;

            StringBuilder builder = new();
            builder.AppendLine($"{username} has used the emojis:");

            IOrderedEnumerable<IGrouping<string, LogEmoji>> groupByEmoji = results
                .GroupBy(row => row.EmojiId)
                .OrderByDescending(g => g.Count());

            foreach (IGrouping<string, LogEmoji> group in groupByEmoji)
            {
                DiscordEmoji emoji = DiscordEmoji.FromName(context.Client, group.Key);
                int totalTimes = group.Count();
                int totalTimesAsReaction = group.Count(emoji => emoji.IsReaction);
                DateTime lastUsed = group
                    .Select(row => row.MessageTimestamp)
                    .OrderByDescending(g => g)
                    .First();

                builder.AppendLine();
                builder.AppendLine($"{emoji}");
                builder.AppendLine($"Used a total of {totalTimes} times ({totalTimesAsReaction} of them as a reaction)");
                builder.AppendLine($"Used last on {lastUsed.ToString("G")}.");

                if (builder.Length >= 1500)
                {
                    await context.RespondAsync(builder.ToString());
                    builder.Clear();
                }
            }

            if (builder.Length > 0)
                await context.RespondAsync(builder.ToString());
        }

        [Command("deleteEmoji")]
        [Description("Deletes all usage of a specific emoji")]
        public async Task DeleteEmojiAsync(CommandContext context, string arg = null)
        {
            try
            {
                _logger.LogInformation($"{context.Message.Author.Username} used the command deleteEmoji with parameters \"{(string.IsNullOrEmpty(arg) ? "" : arg)}\"");
                if (context.Message.Author.Username == _configuration.BotAdminUsername)
                {
                    if (arg != null && arg.StartsWith('<') && arg.EndsWith('>') && arg.Count(c => c == ':') == 2)
                    {
                        string[] parts = arg.Substring(1, arg.Length - 1).Split(':');
                        await _database.DeleteEmojiAsync($":{parts[1]}:");
                    }
                    else
                    {
                        throw new FormatException($"Bad emoji format: {(arg == null ? "null" : arg)}");
                    }
                }
                else
                {
                    await context.Message.RespondAsync($"Hold up! Only {_configuration.BotAdminUsername} can use this command!");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception on command deleteEmoji (arg = \"{(arg == null ? "null" : arg)}\")");
            }
        }

        [Command("logEmojis")]
        [Description("Prints the last X entries on the log table")]
        public async Task LogEmojisAsync(CommandContext context, int? numResults = null)
        {
            try
            {
                _logger.LogInformation($"{context.Message.Author.Username} used the command logEmojis with parameters \"{(numResults == null ? "null" : numResults.Value.ToString())}\"");
                if (context.Message.Author.Username == _configuration.BotAdminUsername)
                {
                    if (numResults == null)
                    {
                        throw new ArgumentNullException("numResults");
                    }

                    if (numResults < -1)
                    {
                        await context.Message.RespondAsync("numResults must be greater than or equal -1");
                    }
                    else
                    {
                        IEnumerable<LogEmoji> emojis = await _database.LogEmojisAsync(numResults.Value);

                        StringBuilder builder = new();
                        builder.AppendLine($"Last {numResults} emojis used:");
                        builder.AppendLine();

                        foreach (LogEmoji emoji in emojis)
                        {
                            DiscordEmoji emojiObj = DiscordEmoji.FromName(context.Client, emoji.EmojiId);
                            builder.AppendLine($"{emoji.MessageTimestamp.ToString("G")}: {emoji.Username} used {emojiObj} {(emoji.IsReaction ? "as a reaction" : "in a message")}");

                            if (builder.Length >= 1500)
                            {
                                await context.RespondAsync(builder.ToString());
                                builder.Clear();
                            }
                        }

                        if (builder.Length > 0)
                            await context.Message.RespondAsync(builder.ToString());
                    }
                }
                else
                {
                    await context.Message.RespondAsync($"Hold up! Only {_configuration.BotAdminUsername} can use this command!");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception on command logEmojis (numResults = \"{(numResults == null ? "null" : numResults.Value.ToString())}\")");
            }
        }

        [Command("harter")]
        [Description("Prints a random Harter quote")]
        public async Task GetRandomHarterQuoteAsync(CommandContext context)
        {
            try
            {
                await _harterQuotesService.GetRandomQuoteAsync(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception on command harter");
            }
        }

        [Command("hoffman")]
        [Description("Prints a random Hoffman quote")]
        public async Task GetRandomHoffmanQuoteAsync(CommandContext context)
        {
            try
            {
                await _hoffmanQuotesService.GetRandomQuoteAsync(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception on command hoffman");
            }
        }

        [Command("version")]
        [Description("Prints the bot version")]
        public async Task GetVersionAsync(CommandContext context)
        {
            try
            {
                _logger.LogInformation($"{context.Message.Author.Username} used the command version");
                Version version = GetType().Assembly.GetName().Version;
                await context.Message.RespondAsync($"Sandbot version {version.Major}.{version.Minor}.{version.Build}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception on command version");
            }
        }
    }
}